using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Mamey.Biometrics.Storage.MinIO;

/// <summary>
/// MinIO implementation for storing biometric images.
/// </summary>
public class BiometricImageStore : IBiometricImageStore
{
    private readonly IMinioClient _minioClient;
    private readonly MinIOOptions _options;
    private readonly ILogger<BiometricImageStore> _logger;

    /// <summary>
    /// Initializes a new instance of the BiometricImageStore.
    /// </summary>
    public BiometricImageStore(
        IMinioClient minioClient,
        IOptions<MinIOOptions> options,
        ILogger<BiometricImageStore> logger)
    {
        _minioClient = minioClient ?? throw new ArgumentNullException(nameof(minioClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<string> UploadImageAsync(Guid templateId, Stream imageStream, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Uploading image for template {TemplateId}", templateId);

            // Ensure bucket exists
            await EnsureBucketExistsAsync(cancellationToken);

            // Generate object ID
            var objectId = GenerateObjectId(templateId, contentType);
            
            // Upload image
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(objectId)
                .WithStreamData(imageStream)
                .WithObjectSize(imageStream.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            _logger.LogInformation("Image uploaded successfully for template {TemplateId}, object ID: {ObjectId}", 
                templateId, objectId);
            
            return objectId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for template {TemplateId}", templateId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Stream> DownloadImageAsync(string objectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Downloading image {ObjectId}", objectId);

            var stream = new MemoryStream();
            
            var getObjectArgs = new GetObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(objectId)
                .WithCallbackStream(s => s.CopyTo(stream));

            await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);
            
            stream.Position = 0;
            
            _logger.LogDebug("Image downloaded successfully: {ObjectId}", objectId);
            return stream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading image {ObjectId}", objectId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteImageAsync(string objectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Deleting image {ObjectId}", objectId);

            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(objectId);

            await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);

            _logger.LogInformation("Image deleted successfully: {ObjectId}", objectId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image {ObjectId}", objectId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ImageExistsAsync(string objectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if image exists: {ObjectId}", objectId);

            var statObjectArgs = new StatObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(objectId);

            await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);
            
            _logger.LogDebug("Image exists: {ObjectId}", objectId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Image does not exist: {ObjectId}", objectId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<ImageMetadata?> GetImageMetadataAsync(string objectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting image metadata: {ObjectId}", objectId);

            var statObjectArgs = new StatObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(objectId);

            var stat = await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);
            
            var metadata = new ImageMetadata
            {
                ObjectId = objectId,
                ContentType = stat.ContentType,
                ContentLength = stat.Size,
                LastModified = stat.LastModified,
                ETag = stat.ETag
            };

            _logger.LogDebug("Image metadata retrieved: {ObjectId}", objectId);
            return metadata;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting image metadata: {ObjectId}", objectId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> EnsureBucketExistsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Ensuring bucket exists: {BucketName}", _options.BucketName);

            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(_options.BucketName);

            var exists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);
            
            if (!exists)
            {
                _logger.LogInformation("Creating bucket: {BucketName}", _options.BucketName);
                
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(_options.BucketName)
                    .WithLocation(_options.Region);

                await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
                
                _logger.LogInformation("Bucket created successfully: {BucketName}", _options.BucketName);
            }
            else
            {
                _logger.LogDebug("Bucket already exists: {BucketName}", _options.BucketName);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ensuring bucket exists: {BucketName}", _options.BucketName);
            return false;
        }
    }

    /// <summary>
    /// Generate a unique object ID for the image.
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="contentType">Content type</param>
    /// <returns>Object ID</returns>
    private string GenerateObjectId(Guid templateId, string contentType)
    {
        var extension = GetFileExtension(contentType);
        var timestamp = DateTime.UtcNow.ToString("yyyy/MM/dd");
        return $"templates/{timestamp}/{templateId}{extension}";
    }

    /// <summary>
    /// Get file extension from content type.
    /// </summary>
    /// <param name="contentType">Content type</param>
    /// <returns>File extension</returns>
    private string GetFileExtension(string contentType)
    {
        return contentType.ToLowerInvariant() switch
        {
            "image/jpeg" or "image/jpg" => ".jpg",
            "image/png" => ".png",
            "image/bmp" => ".bmp",
            "image/tiff" => ".tiff",
            "image/webp" => ".webp",
            _ => ".bin"
        };
    }
}
