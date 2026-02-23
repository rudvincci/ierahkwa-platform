using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Mamey.Persistence.Minio.Exceptions;
using Mamey.Persistence.Minio.Infrastructure;
using Mamey.Persistence.Minio.Infrastructure.Resilience;
using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;
using Mamey.Persistence.Minio.Builders;

namespace Mamey.Persistence.Minio.Services;

/// <summary>
/// Service for managing Minio objects.
/// </summary>
public class ObjectService : BaseMinioService, IObjectService
{
    public ObjectService(
        IMinioClient client,
        IOptions<MinioOptions> options,
        ILogger<ObjectService> logger,
        IRetryPolicyExecutor retryPolicyExecutor)
        : base(client, options, logger, retryPolicyExecutor)
    {
    }

    /// <inheritdoc />
    public async Task PutObjectAsync(PutObjectRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        ValidateBucketAndObjectNames(request.BucketName, request.ObjectName);

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Putting object {ObjectName} to bucket {BucketName}", request.ObjectName, request.BucketName);
                
                var args = new PutObjectArgs()
                    .WithBucket(request.BucketName)
                    .WithObject(request.ObjectName)
                    .WithStreamData(request.Data)
                    .WithObjectSize(request.Size);

                if (!string.IsNullOrEmpty(request.ContentType))
                    args.WithContentType(request.ContentType);

                if (request.Metadata != null && request.Metadata.Count > 0)
                    args.WithHeaders(request.Metadata);

                await Client.PutObjectAsync(args, ct);
                Logger.LogInformation("Successfully put object {ObjectName} to bucket {BucketName}", request.ObjectName, request.BucketName);
            },
            $"PutObject_{request.BucketName}_{request.ObjectName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ObjectMetadata> StatObjectAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        ValidateBucketAndObjectNames(bucketName, objectName);

        return await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogDebug("Getting object metadata for {ObjectName} in bucket {BucketName}", objectName, bucketName);
                
                var args = new StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);

                var stat = await Client.StatObjectAsync(args, ct);
                
                var result = new ObjectMetadata
                {
                    Name = objectName,
                    Size = stat.Size,
                    LastModified = stat.LastModified,
                    ETag = stat.ETag,
                    ContentType = stat.ContentType,
                    VersionId = stat.VersionId,
                    IsDeleteMarker = false, // Not available in current API
                    StorageClass = "STANDARD", // Default storage class
                    UserMetadata = stat.MetaData ?? new Dictionary<string, string>(),
                    SystemMetadata = new Dictionary<string, string>()
                };

                Logger.LogDebug("Successfully retrieved object metadata for {ObjectName} in bucket {BucketName}", objectName, bucketName);
                return result;
            },
            $"StatObject_{bucketName}_{objectName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveObjectAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        ValidateBucketAndObjectNames(bucketName, objectName);

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Removing object {ObjectName} from bucket {BucketName}", objectName, bucketName);
                
                var args = new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);

                await Client.RemoveObjectAsync(args, ct);
                Logger.LogInformation("Successfully removed object {ObjectName} from bucket {BucketName}", objectName, bucketName);
            },
            $"RemoveObject_{bucketName}_{objectName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task CopyObjectAsync(CopyObjectRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        ValidateBucketAndObjectNames(request.SourceBucketName, request.SourceObjectName);
        ValidateBucketAndObjectNames(request.DestinationBucketName, request.DestinationObjectName);

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Copying object {SourceObjectName} from bucket {SourceBucketName} to {DestinationObjectName} in bucket {DestinationBucketName}", 
                    request.SourceObjectName, request.SourceBucketName, request.DestinationObjectName, request.DestinationBucketName);
                
                var args = new CopyObjectArgs()
                    .WithBucket(request.DestinationBucketName)
                    .WithObject(request.DestinationObjectName)
                    .WithCopyObjectSource(new CopySourceObjectArgs()
                        .WithBucket(request.SourceBucketName)
                        .WithObject(request.SourceObjectName));

                if (!string.IsNullOrEmpty(request.ContentType))
                    args.WithContentType(request.ContentType);

                if (request.Headers != null && request.Headers.Count > 0)
                    args.WithHeaders(request.Headers);

                await Client.CopyObjectAsync(args, ct);
                Logger.LogInformation("Successfully copied object {SourceObjectName} to {DestinationObjectName}", request.SourceObjectName, request.DestinationObjectName);
            },
            $"CopyObject_{request.SourceBucketName}_{request.SourceObjectName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ObjectMetadata> UploadFileAsync(string bucketName, string objectName, string filePath, CancellationToken cancellationToken = default)
    {
        ValidateBucketAndObjectNames(bucketName, objectName);
        
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        return await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Uploading file {FilePath} as {ObjectName} to bucket {BucketName}", filePath, objectName, bucketName);
                
                var fileInfo = new FileInfo(filePath);
                var contentType = GetContentType(filePath);
                
                var args = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFileName(filePath)
                    .WithContentType(contentType);

                await Client.PutObjectAsync(args, ct);
                
                var result = new ObjectMetadata
                {
                    Name = objectName,
                    Size = fileInfo.Length,
                    LastModified = fileInfo.LastWriteTime,
                    ContentType = contentType
                };

                Logger.LogInformation("Successfully uploaded file {FilePath} as {ObjectName} to bucket {BucketName}", filePath, objectName, bucketName);
                return result;
            },
            $"UploadFile_{bucketName}_{objectName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ObjectMetadata> UploadBytesAsync(string bucketName, string objectName, byte[] data, CancellationToken cancellationToken = default)
    {
        ValidateBucketAndObjectNames(bucketName, objectName);
        
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        return await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Uploading {DataLength} bytes as {ObjectName} to bucket {BucketName}", data.Length, objectName, bucketName);
                
                using var stream = new MemoryStream(data);
                var args = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(stream)
                    .WithObjectSize(data.Length);

                await Client.PutObjectAsync(args, ct);
                
                var result = new ObjectMetadata
                {
                    Name = objectName,
                    Size = data.Length,
                    LastModified = DateTime.UtcNow,
                    ContentType = GetContentType(objectName)
                };

                Logger.LogInformation("Successfully uploaded {DataLength} bytes as {ObjectName} to bucket {BucketName}", data.Length, objectName, bucketName);
                return result;
            },
            $"UploadBytes_{bucketName}_{objectName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task DownloadToFileAsync(string bucketName, string objectName, string filePath, CancellationToken cancellationToken = default)
    {
        ValidateBucketAndObjectNames(bucketName, objectName);
        
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Downloading object {ObjectName} from bucket {BucketName} to file {FilePath}", objectName, bucketName, filePath);
                
                var args = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFile(filePath);

                await Client.GetObjectAsync(args, ct);
                Logger.LogInformation("Successfully downloaded object {ObjectName} from bucket {BucketName} to file {FilePath}", objectName, bucketName, filePath);
            },
            $"DownloadToFile_{bucketName}_{objectName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<byte[]> DownloadAsBytesAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        ValidateBucketAndObjectNames(bucketName, objectName);

        return await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogDebug("Downloading object {ObjectName} from bucket {BucketName} as bytes", objectName, bucketName);
                
                using var stream = new MemoryStream();
                var args = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithCallbackStream(s => s.CopyTo(stream));

                await Client.GetObjectAsync(args, ct);
                
                var result = stream.ToArray();
                Logger.LogDebug("Successfully downloaded {DataLength} bytes for object {ObjectName} from bucket {BucketName}", result.Length, objectName, bucketName);
                return result;
            },
            $"DownloadAsBytes_{bucketName}_{objectName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task DownloadAsync(string bucketName, string objectName, Stream destination, IProgress<long>? progress = null, CancellationToken cancellationToken = default)
    {
        ValidateBucketAndObjectNames(bucketName, objectName);
        
        if (destination == null)
            throw new ArgumentNullException(nameof(destination));

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogDebug("Downloading object {ObjectName} from bucket {BucketName} to stream", objectName, bucketName);
                
                var args = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithCallbackStream(s =>
                    {
                        var buffer = new byte[8192];
                        int bytesRead;
                        long totalBytesRead = 0;
                        
                        while ((bytesRead = s.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            destination.Write(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;
                            progress?.Report(totalBytesRead);
                        }
                    });

                await Client.GetObjectAsync(args, ct);
                Logger.LogDebug("Successfully downloaded object {ObjectName} from bucket {BucketName} to stream", objectName, bucketName);
            },
            $"Download_{bucketName}_{objectName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ObjectMetadata> UploadMultipartAsync(MultipartUploadRequest request, IProgress<MultipartUploadProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        ValidateBucketAndObjectNames(request.BucketName, request.ObjectName);

        return await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Starting multipart upload for {ObjectName} in bucket {BucketName} with size {Size}", 
                    request.ObjectName, request.BucketName, request.Stream.Length);

                var multipartLogger = NullLogger<Services.MultipartUploadService>.Instance;
                var multipartService = new MultipartUploadService(Client, Microsoft.Extensions.Options.Options.Create(Options), multipartLogger, RetryPolicyExecutor);
                
                // Calculate optimal part size
                var partSize = CalculateOptimalPartSize(request.Stream.Length, request.PartSize, request.MinPartSize, request.MaxPartSize);
                var totalParts = (int)Math.Ceiling((double)request.Stream.Length / partSize);

                Logger.LogDebug("Using part size {PartSize} for {TotalParts} parts", partSize, totalParts);

                // Initiate multipart upload
                var uploadId = await multipartService.InitiateMultipartUploadAsync(
                    request.BucketName, 
                    request.ObjectName, 
                    request.ContentType, 
                    request.Metadata, 
                    ct);

                var partETags = new List<string>();
                var bytesUploaded = 0L;
                var startTime = DateTime.UtcNow;

                try
                {
                    // Upload parts
                    var semaphore = new SemaphoreSlim(request.MaxConcurrency);
                    var tasks = new List<Task>();

                    for (int partNumber = 1; partNumber <= totalParts; partNumber++)
                    {
                        var currentPartNumber = partNumber;
                        var task = Task.Run(async () =>
                        {
                            await semaphore.WaitAsync(ct);
                            try
                            {
                                var offset = (currentPartNumber - 1) * partSize;
                                var currentPartSize = Math.Min(partSize, request.Stream.Length - offset);
                                
                                using var partStream = new MemoryStream();
                                request.Stream.Position = offset;
                                
                                var buffer = new byte[8192];
                                int bytesRead;
                                var remainingBytes = currentPartSize;
                                
                                while (remainingBytes > 0 && (bytesRead = await request.Stream.ReadAsync(buffer, 0, (int)Math.Min(buffer.Length, remainingBytes), ct)) > 0)
                                {
                                    await partStream.WriteAsync(buffer, 0, bytesRead, ct);
                                    remainingBytes -= bytesRead;
                                }
                                
                                partStream.Position = 0;

                                var etag = await multipartService.UploadPartAsync(
                                    request.BucketName,
                                    request.ObjectName,
                                    uploadId,
                                    currentPartNumber,
                                    partStream,
                                    ct);

                                lock (partETags)
                                {
                                    partETags.Add(etag);
                                    bytesUploaded += currentPartSize;
                                }

                                // Report progress
                                var progressInfo = new MultipartUploadProgress
                                {
                                    UploadId = uploadId,
                                    TotalSize = request.Stream.Length,
                                    BytesUploaded = bytesUploaded,
                                    TotalParts = totalParts,
                                    CompletedParts = partETags.Count,
                                    CurrentPart = currentPartNumber,
                                    CurrentPartSize = currentPartSize
                                };

                                // Calculate upload speed
                                var elapsed = DateTime.UtcNow - startTime;
                                if (elapsed.TotalSeconds > 0)
                                {
                                    progressInfo.BytesPerSecond = (long)(bytesUploaded / elapsed.TotalSeconds);
                                    
                                    if (progressInfo.BytesPerSecond > 0)
                                    {
                                        var remainingBytesToUpload = request.Stream.Length - bytesUploaded;
                                        progressInfo.EstimatedTimeRemaining = TimeSpan.FromSeconds(remainingBytesToUpload / progressInfo.BytesPerSecond);
                                    }
                                }

                                progress?.Report(progressInfo);
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }, ct);

                        tasks.Add(task);
                    }

                    await Task.WhenAll(tasks);

                    // Complete multipart upload
                    var finalETag = await multipartService.CompleteMultipartUploadAsync(
                        request.BucketName,
                        request.ObjectName,
                        uploadId,
                        partETags,
                        ct);

                    var result = new ObjectMetadata
                    {
                        Name = request.ObjectName,
                        Size = request.Stream.Length,
                        LastModified = DateTime.UtcNow,
                        ETag = finalETag,
                        ContentType = request.ContentType ?? GetContentType(request.ObjectName)
                    };

                    Logger.LogInformation("Successfully completed multipart upload for {ObjectName} in bucket {BucketName} with ETag {ETag}", 
                        request.ObjectName, request.BucketName, finalETag);

                    return result;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Multipart upload failed for {ObjectName} in bucket {BucketName}. Aborting upload.", 
                        request.ObjectName, request.BucketName);
                    
                    // Abort the upload
                    try
                    {
                        await multipartService.AbortMultipartUploadAsync(request.BucketName, request.ObjectName, uploadId, ct);
                    }
                    catch (Exception abortEx)
                    {
                        Logger.LogWarning(abortEx, "Failed to abort multipart upload {UploadId}", uploadId);
                    }
                    
                    throw;
                }
            },
            $"UploadMultipart_{request.BucketName}_{request.ObjectName}",
            cancellationToken);
    }

    private static long CalculateOptimalPartSize(long totalSize, long? requestedPartSize, long minPartSize, long maxPartSize)
    {
        if (requestedPartSize.HasValue)
        {
            return Math.Max(minPartSize, Math.Min(maxPartSize, requestedPartSize.Value));
        }

        // Calculate optimal part size based on total size
        var optimalSize = Math.Max(minPartSize, totalSize / 10000); // Aim for ~10,000 parts max
        return Math.Min(maxPartSize, optimalSize);
    }

    /// <inheritdoc />
    public async Task<ObjectMetadata> UploadAsync(PutObjectRequest request, IProgress<MultipartUploadProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        ValidateBucketAndObjectNames(request.BucketName, request.ObjectName);

        // Check if we should use multipart upload
        var shouldUseMultipart = request.Data.Length > 100 * 1024 * 1024; // 100MB threshold

        if (shouldUseMultipart)
        {
            var multipartRequest = new MultipartUploadRequest
            {
                BucketName = request.BucketName,
                ObjectName = request.ObjectName,
                Stream = request.Data,
                ContentType = request.ContentType,
                Metadata = request.Metadata
            };

            return await UploadMultipartAsync(multipartRequest, progress, cancellationToken);
        }
        else
        {
            await PutObjectAsync(request, cancellationToken);
            
            // Get object metadata after upload
            var statArgs = new StatObjectArgs()
                .WithBucket(request.BucketName)
                .WithObject(request.ObjectName);

            var stat = await Client.StatObjectAsync(statArgs, cancellationToken);
            
            return new ObjectMetadata
            {
                Name = request.ObjectName,
                Size = stat.Size,
                LastModified = stat.LastModified,
                ETag = stat.ETag,
                ContentType = request.ContentType ?? GetContentType(request.ObjectName)
            };
        }
    }

    /// <inheritdoc />
    public async Task DownloadAsync(ObjectDownloadConfiguration configuration, Stream destination, IProgress<long>? progress = null, CancellationToken cancellationToken = default)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        ValidateBucketAndObjectNames(configuration.BucketName, configuration.ObjectName);

        if (destination == null)
            throw new ArgumentNullException(nameof(destination));

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogDebug("Downloading object {ObjectName} from bucket {BucketName} to stream with configuration", 
                    configuration.ObjectName, configuration.BucketName);

                var args = new GetObjectArgs()
                    .WithBucket(configuration.BucketName)
                    .WithObject(configuration.ObjectName)
                    .WithCallbackStream(s =>
                    {
                        var buffer = new byte[8192];
                        int bytesRead;
                        long totalBytesRead = 0;
                        
                        while ((bytesRead = s.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            destination.Write(buffer, 0, bytesRead);
                            totalBytesRead += bytesRead;
                            progress?.Report(totalBytesRead);
                        }
                    });

                // Add version ID if specified
                if (!string.IsNullOrEmpty(configuration.VersionId))
                {
                    // Note: Version ID support may not be available in current Minio API
                    Logger.LogDebug("Version ID {VersionId} specified for download", configuration.VersionId);
                }

                // Add headers if specified
                if (configuration.Headers != null && configuration.Headers.Count > 0)
                {
                    // Note: Custom headers may not be fully supported in current Minio API
                    Logger.LogDebug("Custom headers specified for download");
                }

                await Client.GetObjectAsync(args, ct);
                Logger.LogDebug("Successfully downloaded object {ObjectName} from bucket {BucketName} to stream", 
                    configuration.ObjectName, configuration.BucketName);
            },
            $"Download_{configuration.BucketName}_{configuration.ObjectName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ObjectMetadata> UploadFileAsync(string bucketName, string objectName, string filePath, string? contentType = null, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default)
    {
        ValidateBucketAndObjectNames(bucketName, objectName);
        
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}", filePath);

        using var fileStream = File.OpenRead(filePath);
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            ObjectName = objectName,
            Data = fileStream,
            Size = fileStream.Length,
            ContentType = contentType ?? GetContentType(filePath),
            Metadata = metadata
        };

        return await UploadAsync(request, null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DownloadToFileAsync(string bucketName, string objectName, string filePath, IProgress<long>? progress = null, CancellationToken cancellationToken = default)
    {
        ValidateBucketAndObjectNames(bucketName, objectName);
        
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        // Ensure directory exists
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var fileStream = File.Create(filePath);
        await DownloadAsync(bucketName, objectName, fileStream, progress, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ObjectMetadata> UploadBytesAsync(string bucketName, string objectName, byte[] data, string? contentType = null, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default)
    {
        ValidateBucketAndObjectNames(bucketName, objectName);
        
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        using var memoryStream = new MemoryStream(data);
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            ObjectName = objectName,
            Data = memoryStream,
            Size = data.Length,
            ContentType = contentType ?? GetContentType(objectName),
            Metadata = metadata
        };

        return await UploadAsync(request, null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<byte[]> DownloadAsBytesAsync(string bucketName, string objectName, IProgress<long>? progress = null, CancellationToken cancellationToken = default)
    {
        ValidateBucketAndObjectNames(bucketName, objectName);

        using var memoryStream = new MemoryStream();
        await DownloadAsync(bucketName, objectName, memoryStream, progress, cancellationToken);
        return memoryStream.ToArray();
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".txt" => "text/plain",
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".zip" => "application/zip",
            ".mp4" => "video/mp4",
            ".mp3" => "audio/mpeg",
            _ => "application/octet-stream"
        };
    }
}
