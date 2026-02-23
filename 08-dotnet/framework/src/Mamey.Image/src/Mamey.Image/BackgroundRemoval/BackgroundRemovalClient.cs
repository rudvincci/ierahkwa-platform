using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Http;
using Mamey.Image.BackgroundRemoval.Models;

namespace Mamey.Image.BackgroundRemoval;

/// <summary>
/// HTTP client for background removal API service.
/// </summary>
public class BackgroundRemovalClient : IBackgroundRemovalClient
{
    private readonly HttpClient _httpClient;
    private readonly BackgroundRemovalOptions _options;
    private readonly ILogger<BackgroundRemovalClient> _logger;

    /// <summary>
    /// Initializes a new instance of the BackgroundRemovalClient.
    /// </summary>
    public BackgroundRemovalClient(
        HttpClient httpClient,
        IOptions<BackgroundRemovalOptions> options,
        ILogger<BackgroundRemovalClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Configure base URL
        if (!_options.BaseUrl.EndsWith("/"))
            _options.BaseUrl += "/";
        
        _httpClient.BaseAddress = new Uri(_options.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
    }

    /// <inheritdoc/>
    public async Task<Stream> RemoveBackgroundAsync(Stream imageStream, string outputFormat = "PNG", CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting background removal for single image");

            // Validate input
            if (imageStream == null || imageStream.Length == 0)
                throw new ArgumentException("Image stream cannot be null or empty", nameof(imageStream));

            if (imageStream.Length > _options.MaxFileSizeBytes)
                throw new ArgumentException($"Image size exceeds maximum allowed size of {_options.MaxFileSizeBytes} bytes");

            // Prepare multipart content
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(imageStream);
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
            
            content.Add(streamContent, "file", "image.png");
            content.Add(new StringContent(outputFormat), "output_format");

            // Make request
            var response = await _httpClient.PostAsync("/api/remove-background", content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Background removal failed: {response.StatusCode} - {errorContent}");
            }

            _logger.LogInformation("Background removal completed successfully");
            return await response.Content.ReadAsStreamAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during background removal");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<byte[]> RemoveBackgroundFromBytesAsync(byte[] imageBytes, string outputFormat = "PNG", CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting background removal for raw image bytes");

            // Validate input
            if (imageBytes == null || imageBytes.Length == 0)
                throw new ArgumentException("Image bytes cannot be null or empty", nameof(imageBytes));

            if (imageBytes.Length > _options.MaxFileSizeBytes)
                throw new ArgumentException($"Image size exceeds maximum allowed size of {_options.MaxFileSizeBytes} bytes");

            // Prepare JSON content with raw bytes
            var request = new
            {
                image_data = Convert.ToBase64String(imageBytes),
                output_format = outputFormat
            };

            var jsonContent = JsonSerializer.Serialize(request);
            using var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Make request to the bytes endpoint
            var response = await _httpClient.PostAsync("/api/remove-background/bytes", content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Background removal failed: {response.StatusCode} - {errorContent}");
            }

            _logger.LogInformation("Background removal completed successfully for raw bytes");
            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during background removal for raw bytes");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<string> RemoveBackgroundFromFileAsync(string filePath, string? outputPath = null, string outputFormat = "PNG", CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting background removal for file: {FilePath}", filePath);

            // Validate input file
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Input file not found: {filePath}");

            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > _options.MaxFileSizeBytes)
                throw new ArgumentException($"File size exceeds maximum allowed size of {_options.MaxFileSizeBytes} bytes");

            // Generate output path if not provided
            if (string.IsNullOrEmpty(outputPath))
            {
                var directory = Path.GetDirectoryName(filePath) ?? "";
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var extension = outputFormat.ToLowerInvariant();
                outputPath = Path.Combine(directory, $"{fileName}_no_bg.{extension}");
            }

            // Process file
            using var inputStream = File.OpenRead(filePath);
            using var outputStream = await RemoveBackgroundAsync(inputStream, outputFormat, cancellationToken);
            
            // Save result
            using var fileStream = File.Create(outputPath);
            await outputStream.CopyToAsync(fileStream, cancellationToken);

            _logger.LogInformation("Background removal completed successfully. Output: {OutputPath}", outputPath);
            return outputPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during file background removal: {FilePath}", filePath);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Stream> RemoveBackgroundBatchAsync(string[] filePaths, string outputFormat = "PNG", CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting batch background removal for {Count} files", filePaths.Length);

            // Validate input
            if (filePaths == null || filePaths.Length == 0)
                throw new ArgumentException("File paths cannot be null or empty", nameof(filePaths));

            if (filePaths.Length > _options.MaxBatchSize)
                throw new ArgumentException($"Batch size exceeds maximum allowed size of {_options.MaxBatchSize} files");

            // Validate all files exist
            foreach (var filePath in filePaths)
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Input file not found: {filePath}");
            }

            // Prepare multipart content
            using var content = new MultipartFormDataContent();
            
            foreach (var filePath in filePaths)
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > _options.MaxFileSizeBytes)
                {
                    _logger.LogWarning("Skipping file {FilePath} - size exceeds maximum allowed size", filePath);
                    continue;
                }

                using var fileStream = File.OpenRead(filePath);
                var streamContent = new StreamContent(fileStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                
                content.Add(streamContent, "files", Path.GetFileName(filePath));
            }

            content.Add(new StringContent(outputFormat), "output_format");

            // Make request
            var response = await _httpClient.PostAsync("/api/remove-background/batch", content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Batch background removal failed: {response.StatusCode} - {errorContent}");
            }

            _logger.LogInformation("Batch background removal completed successfully");
            return await response.Content.ReadAsStreamAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during batch background removal");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<string[]> GetAvailableModelsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving available models");

            var response = await _httpClient.GetAsync("/api/models", cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"Failed to get models: {response.StatusCode} - {errorContent}");
            }
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var modelsResponse = JsonSerializer.Deserialize<ModelsResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            _logger.LogInformation("Retrieved {Count} available models", modelsResponse?.Models.Length ?? 0);
            return modelsResponse?.Models ?? Array.Empty<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available models");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking service health");

            var response = await _httpClient.GetAsync("/api/health", cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Health check failed with status: {StatusCode}", response.StatusCode);
                return false;
            }
            
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var healthResponse = JsonSerializer.Deserialize<HealthCheckResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            var isHealthy = healthResponse?.Status.Equals("healthy", StringComparison.OrdinalIgnoreCase) ?? false;
            _logger.LogDebug("Service health check result: {IsHealthy}", isHealthy);
            
            return isHealthy;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Service health check failed");
            return false;
        }
    }

}
