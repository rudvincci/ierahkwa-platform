using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Biometrics.Engine;
using Mamey.Biometrics.Engine.Models;
using Mamey.Biometrics.Services.Models;
using Mamey.Biometrics.Storage.MongoDB;
using Mamey.Biometrics.Storage.MongoDB.Models;
using Mamey.Biometrics.Storage.MinIO;

namespace Mamey.Biometrics.Services;

/// <summary>
/// Main biometric service implementation.
/// </summary>
public class BiometricService : IBiometricService
{
    private readonly IBiometricEngineClient _engineClient;
    private readonly IBiometricTemplateRepository _templateRepository;
    private readonly IBiometricImageStore _imageStore;
    private readonly IMemoryCache _cache;
    private readonly BiometricsOptions _options;
    private readonly ILogger<BiometricService> _logger;

    private const string CacheKeyPrefix = "biometric_template_";
    private const int CacheExpirationMinutes = 15;

    /// <summary>
    /// Initializes a new instance of the BiometricService.
    /// </summary>
    public BiometricService(
        IBiometricEngineClient engineClient,
        IBiometricTemplateRepository templateRepository,
        IBiometricImageStore imageStore,
        IMemoryCache cache,
        IOptions<BiometricsOptions> options,
        ILogger<BiometricService> logger)
    {
        _engineClient = engineClient ?? throw new ArgumentNullException(nameof(engineClient));
        _templateRepository = templateRepository ?? throw new ArgumentNullException(nameof(templateRepository));
        _imageStore = imageStore ?? throw new ArgumentNullException(nameof(imageStore));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<EnrollmentResult> EnrollAsync(EnrollmentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting enrollment for subject {SubjectId}", request.SubjectId);

            // Validate image size
            var imageBytes = Convert.FromBase64String(request.ImageData);
            if (imageBytes.Length > _options.MaxImageSizeBytes)
            {
                return new EnrollmentResult
                {
                    Success = false,
                    SubjectId = request.SubjectId,
                    Error = "Image size exceeds maximum allowed size",
                    Message = $"Image size {imageBytes.Length} bytes exceeds maximum {_options.MaxImageSizeBytes} bytes"
                };
            }

            // Extract face encoding using Python engine
            var extractRequest = new ExtractEncodingRequest
            {
                ImageData = request.ImageData,
                ImageFormat = request.ImageFormat
            };

            var extractResponse = await _engineClient.ExtractEncodingAsync(extractRequest, cancellationToken);
            
            if (!extractResponse.Success || extractResponse.Encoding == null)
            {
                return new EnrollmentResult
                {
                    Success = false,
                    SubjectId = request.SubjectId,
                    Error = extractResponse.Error ?? "Face encoding extraction failed",
                    Message = extractResponse.Message
                };
            }

            // Check quality score
            var qualityScore = extractResponse.QualityScore ?? 0.0;
            var minQuality = request.MinQualityScore ?? 0.5; // Default minimum quality
            
            if (qualityScore < minQuality)
            {
                return new EnrollmentResult
                {
                    Success = false,
                    SubjectId = request.SubjectId,
                    Error = "Face quality too low",
                    Message = $"Face quality score {qualityScore:F3} is below minimum required {minQuality:F3}"
                };
            }

            // Generate template ID
            var templateId = Guid.NewGuid();

            // Upload original image to MinIO
            var imageStream = new MemoryStream(imageBytes);
            var contentType = GetContentType(request.ImageFormat);
            var objectId = await _imageStore.UploadImageAsync(templateId, imageStream, contentType, cancellationToken);

            // Create template document
            var template = new BiometricTemplateDocument
            {
                TemplateId = templateId,
                SubjectId = request.SubjectId,
                Encoding = extractResponse.Encoding,
                Metadata = new Storage.MongoDB.Models.BiometricTemplateMetadata
                {
                    QualityScore = qualityScore,
                    ImageFormat = request.ImageFormat,
                    ImageSizeBytes = imageBytes.Length,
                    FaceLocation = extractResponse.FaceLocation != null ? new Storage.MongoDB.Models.FaceLocation
                    {
                        Top = extractResponse.FaceLocation.Top,
                        Right = extractResponse.FaceLocation.Right,
                        Bottom = extractResponse.FaceLocation.Bottom,
                        Left = extractResponse.FaceLocation.Left
                    } : null,
                    CustomData = request.CustomData
                },
                MinioObjectId = objectId,
                Tags = request.Tags
            };

            // Save template to MongoDB
            await _templateRepository.AddAsync(template, cancellationToken);

            _logger.LogInformation("Enrollment completed successfully for subject {SubjectId}, template {TemplateId}", 
                request.SubjectId, templateId);

            return new EnrollmentResult
            {
                Success = true,
                TemplateId = templateId,
                SubjectId = request.SubjectId,
                QualityScore = qualityScore,
                FaceLocation = extractResponse.FaceLocation != null ? new Services.Models.FaceLocation
                {
                    Top = extractResponse.FaceLocation.Top,
                    Right = extractResponse.FaceLocation.Right,
                    Bottom = extractResponse.FaceLocation.Bottom,
                    Left = extractResponse.FaceLocation.Left
                } : null,
                Message = "Template enrolled successfully",
                EnrolledAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during enrollment for subject {SubjectId}", request.SubjectId);
            return new EnrollmentResult
            {
                Success = false,
                SubjectId = request.SubjectId,
                Error = ex.Message,
                Message = "Enrollment failed due to an internal error"
            };
        }
    }

    /// <inheritdoc/>
    public async Task<VerificationResult> VerifyAsync(VerificationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting verification for template {TemplateId}", request.TemplateId);

            // Get template from cache or database
            var template = await GetTemplateFromCacheOrDatabaseAsync(request.TemplateId, cancellationToken);
            if (template == null)
            {
                return new VerificationResult
                {
                    Success = false,
                    TemplateId = request.TemplateId,
                    Error = "Template not found",
                    Message = $"Template {request.TemplateId} not found"
                };
            }

            // Extract encoding from probe image
            var extractRequest = new ExtractEncodingRequest
            {
                ImageData = request.ImageData,
                ImageFormat = request.ImageFormat
            };

            var extractResponse = await _engineClient.ExtractEncodingAsync(extractRequest, cancellationToken);
            
            if (!extractResponse.Success || extractResponse.Encoding == null)
            {
                return new VerificationResult
                {
                    Success = false,
                    TemplateId = request.TemplateId,
                    SubjectId = template.SubjectId,
                    Error = extractResponse.Error ?? "Face encoding extraction failed",
                    Message = extractResponse.Message
                };
            }

            // Compare encodings
            var compareRequest = new CompareEncodingsRequest
            {
                Encoding1 = template.Encoding,
                Encoding2 = extractResponse.Encoding
            };

            var compareResponse = await _engineClient.CompareEncodingsAsync(compareRequest, cancellationToken);
            
            if (!compareResponse.Success)
            {
                return new VerificationResult
                {
                    Success = false,
                    TemplateId = request.TemplateId,
                    SubjectId = template.SubjectId,
                    Error = compareResponse.Error ?? "Face comparison failed",
                    Message = compareResponse.Message
                };
            }

            // Apply threshold
            var threshold = request.Threshold ?? _options.DefaultVerificationThreshold;
            var match = compareResponse.Similarity >= threshold;

            _logger.LogInformation("Verification completed for template {TemplateId}, match: {Match}, similarity: {Similarity:F3}", 
                request.TemplateId, match, compareResponse.Similarity);

            return new VerificationResult
            {
                Success = true,
                Match = match,
                TemplateId = request.TemplateId,
                SubjectId = template.SubjectId,
                Similarity = compareResponse.Similarity ?? 0.0,
                Distance = compareResponse.Distance ?? 0.0,
                Threshold = threshold,
                ProbeQualityScore = extractResponse.QualityScore,
                Message = match ? "Faces match" : "Faces do not match",
                VerifiedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during verification for template {TemplateId}", request.TemplateId);
            return new VerificationResult
            {
                Success = false,
                TemplateId = request.TemplateId,
                Error = ex.Message,
                Message = "Verification failed due to an internal error"
            };
        }
    }

    /// <inheritdoc/>
    public async Task<IdentificationResult> IdentifyAsync(IdentificationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting identification with max results {MaxResults}", request.MaxResults);

            // Extract encoding from probe image
            var extractRequest = new ExtractEncodingRequest
            {
                ImageData = request.ImageData,
                ImageFormat = request.ImageFormat
            };

            var extractResponse = await _engineClient.ExtractEncodingAsync(extractRequest, cancellationToken);
            
            if (!extractResponse.Success || extractResponse.Encoding == null)
            {
                return new IdentificationResult
                {
                    Success = false,
                    Error = extractResponse.Error ?? "Face encoding extraction failed",
                    Message = extractResponse.Message
                };
            }

            // Get templates to search against
            var templates = await GetTemplatesForIdentificationAsync(request, cancellationToken);
            
            if (!templates.Any())
            {
                return new IdentificationResult
                {
                    Success = true,
                    Matches = new List<BiometricMatch>(),
                    TemplatesSearched = 0,
                    Threshold = request.Threshold ?? _options.DefaultIdentificationThreshold,
                    ProbeQualityScore = extractResponse.QualityScore,
                    Message = "No templates found to search against",
                    IdentifiedAt = DateTime.UtcNow
                };
            }

            // Compare against all templates
            var matches = new List<BiometricMatch>();
            var threshold = request.Threshold ?? _options.DefaultIdentificationThreshold;

            foreach (var template in templates)
            {
                var compareRequest = new CompareEncodingsRequest
                {
                    Encoding1 = template.Encoding,
                    Encoding2 = extractResponse.Encoding
                };

                var compareResponse = await _engineClient.CompareEncodingsAsync(compareRequest, cancellationToken);
                
                if (compareResponse.Success && compareResponse.Similarity >= threshold)
                {
                    matches.Add(new BiometricMatch
                    {
                        TemplateId = template.TemplateId,
                        SubjectId = template.SubjectId,
                        Similarity = compareResponse.Similarity ?? 0.0,
                        Distance = compareResponse.Distance ?? 0.0,
                        TemplateQualityScore = template.Metadata.QualityScore,
                        CreatedAt = template.CreatedAt,
                        Tags = template.Tags
                    });
                }
            }

            // Sort by similarity (highest first) and limit results
            var sortedMatches = matches
                .OrderByDescending(m => m.Similarity)
                .Take(request.MaxResults)
                .ToList();

            _logger.LogInformation("Identification completed, found {MatchCount} matches out of {TemplateCount} templates searched", 
                sortedMatches.Count, templates.Count);

            return new IdentificationResult
            {
                Success = true,
                Matches = sortedMatches,
                TemplatesSearched = templates.Count,
                Threshold = threshold,
                ProbeQualityScore = extractResponse.QualityScore,
                Message = $"Found {sortedMatches.Count} matching templates",
                IdentifiedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during identification");
            return new IdentificationResult
            {
                Success = false,
                Error = ex.Message,
                Message = "Identification failed due to an internal error"
            };
        }
    }

    /// <inheritdoc/>
    public async Task<BiometricTemplate?> GetTemplateAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting template {TemplateId}", templateId);

            var template = await GetTemplateFromCacheOrDatabaseAsync(templateId, cancellationToken);
            if (template == null)
            {
                return null;
            }

            return ConvertToBiometricTemplate(template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting template {TemplateId}", templateId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Services.Models.PagedResult<BiometricTemplate>> GetTemplatesAsync(int page = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting templates page {Page}, size {PageSize}", page, pageSize);

            var pagedResult = await _templateRepository.GetPagedAsync(page, pageSize, cancellationToken);
            
            var templates = pagedResult.Items.Select(ConvertToBiometricTemplate).ToList();

            return new Services.Models.PagedResult<BiometricTemplate>
            {
                Items = templates,
                TotalCount = pagedResult.TotalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting templates");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteTemplateAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting template {TemplateId}", templateId);

            // Get template to find MinIO object ID
            var template = await _templateRepository.GetByIdAsync(templateId, cancellationToken);
            if (template == null)
            {
                _logger.LogWarning("Template {TemplateId} not found for deletion", templateId);
                return false;
            }

            // Delete from MongoDB
            var deleted = await _templateRepository.DeleteAsync(templateId, cancellationToken);
            if (!deleted)
            {
                return false;
            }

            // Delete from MinIO
            await _imageStore.DeleteImageAsync(template.MinioObjectId, cancellationToken);

            // Remove from cache
            var cacheKey = $"{CacheKeyPrefix}{templateId}";
            _cache.Remove(cacheKey);

            _logger.LogInformation("Template {TemplateId} deleted successfully", templateId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting template {TemplateId}", templateId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<List<BiometricTemplate>> GetTemplatesBySubjectAsync(Guid subjectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting templates for subject {SubjectId}", subjectId);

            var templates = await _templateRepository.GetBySubjectIdAsync(subjectId, cancellationToken);
            return templates.Select(ConvertToBiometricTemplate).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting templates for subject {SubjectId}", subjectId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Stream?> GetOriginalImageAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting original image for template {TemplateId}", templateId);

            var template = await _templateRepository.GetByIdAsync(templateId, cancellationToken);
            if (template == null)
            {
                return null;
            }

            return await _imageStore.DownloadImageAsync(template.MinioObjectId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting original image for template {TemplateId}", templateId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteImageAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting image for template {TemplateId}", templateId);

            var template = await _templateRepository.GetByIdAsync(templateId, cancellationToken);
            if (template == null)
            {
                return false;
            }

            return await _imageStore.DeleteImageAsync(template.MinioObjectId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image for template {TemplateId}", templateId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Check Python engine health
            var engineHealthy = await _engineClient.IsHealthyAsync(cancellationToken);
            
            // Check database health (simple count query)
            var dbHealthy = true;
            try
            {
                await _templateRepository.GetCountAsync(cancellationToken);
            }
            catch
            {
                dbHealthy = false;
            }

            // Check MinIO health (bucket exists check)
            var storageHealthy = await _imageStore.EnsureBucketExistsAsync(cancellationToken);

            return engineHealthy && dbHealthy && storageHealthy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<ServiceStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var totalTemplates = await _templateRepository.GetCountAsync(cancellationToken);
            var engineHealthy = await _engineClient.IsHealthyAsync(cancellationToken);
            var storageHealthy = await _imageStore.EnsureBucketExistsAsync(cancellationToken);

            // Get average quality score (simplified - in production you might want to calculate this properly)
            var templates = await _templateRepository.GetPagedAsync(1, 1000, cancellationToken);
            var averageQuality = templates.Items.Any() 
                ? templates.Items.Average(t => t.Metadata.QualityScore) 
                : 0.0;

            // Count unique subjects
            var uniqueSubjects = templates.Items.Select(t => t.SubjectId).Distinct().Count();

            return new ServiceStatistics
            {
                TotalTemplates = totalTemplates,
                TotalSubjects = uniqueSubjects,
                AverageQualityScore = averageQuality,
                EngineHealthy = engineHealthy,
                DatabaseHealthy = true, // We got here, so DB is working
                StorageHealthy = storageHealthy,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting statistics");
            throw;
        }
    }

    #region Private Helper Methods

    private async Task<BiometricTemplateDocument?> GetTemplateFromCacheOrDatabaseAsync(Guid templateId, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeyPrefix}{templateId}";
        
        if (_cache.TryGetValue(cacheKey, out BiometricTemplateDocument? cachedTemplate))
        {
            _logger.LogDebug("Template {TemplateId} found in cache", templateId);
            return cachedTemplate;
        }

        var template = await _templateRepository.GetByIdAsync(templateId, cancellationToken);
        if (template != null)
        {
            _cache.Set(cacheKey, template, TimeSpan.FromMinutes(CacheExpirationMinutes));
            _logger.LogDebug("Template {TemplateId} cached", templateId);
        }

        return template;
    }

    private async Task<List<BiometricTemplateDocument>> GetTemplatesForIdentificationAsync(IdentificationRequest request, CancellationToken cancellationToken)
    {
        // Apply filters if specified
        if (request.SubjectId.HasValue || request.Tags?.Any() == true || request.MinQualityScore.HasValue)
        {
            return await _templateRepository.SearchAsync(
                subjectId: request.SubjectId,
                tags: request.Tags?.ToArray(),
                minQualityScore: request.MinQualityScore,
                cancellationToken: cancellationToken);
        }

        // Get all templates if no filters
        return await _templateRepository.GetAllAsync(cancellationToken);
    }

    private BiometricTemplate ConvertToBiometricTemplate(BiometricTemplateDocument document)
    {
        return new BiometricTemplate
        {
            TemplateId = document.TemplateId,
            SubjectId = document.SubjectId,
            Metadata = new Services.Models.BiometricTemplateMetadata
            {
                QualityScore = document.Metadata.QualityScore,
                ImageFormat = document.Metadata.ImageFormat,
                ImageSizeBytes = document.Metadata.ImageSizeBytes,
                FaceLocation = document.Metadata.FaceLocation != null ? new Services.Models.FaceLocation
                {
                    Top = document.Metadata.FaceLocation.Top,
                    Right = document.Metadata.FaceLocation.Right,
                    Bottom = document.Metadata.FaceLocation.Bottom,
                    Left = document.Metadata.FaceLocation.Left
                } : null,
                CustomData = document.Metadata.CustomData
            },
            MinioObjectId = document.MinioObjectId,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt,
            Tags = document.Tags
        };
    }

    private string GetContentType(string imageFormat)
    {
        return imageFormat.ToUpperInvariant() switch
        {
            "JPEG" or "JPG" => "image/jpeg",
            "PNG" => "image/png",
            "BMP" => "image/bmp",
            "TIFF" => "image/tiff",
            "WEBP" => "image/webp",
            _ => "application/octet-stream"
        };
    }

    #endregion
}
