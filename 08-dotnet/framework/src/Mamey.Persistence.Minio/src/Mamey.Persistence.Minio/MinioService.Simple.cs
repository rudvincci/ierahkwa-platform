using System.Collections.ObjectModel;
using Mamey.Exceptions;
using Mamey.Persistence.Minio.Exceptions;
using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Minio.Exceptions;

namespace Mamey.Persistence.Minio;

/// <summary>
/// Simplified Minio service that works with the current Minio API.
/// </summary>
public class SimpleMinioService
{
    private readonly IMinioClient _client;
    private readonly MinioOptions _options;
    private readonly ILogger<SimpleMinioService> _logger;

    public SimpleMinioService(IMinioClient client, IOptions<MinioOptions> options, ILogger<SimpleMinioService> logger)
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Lists all buckets.
    /// </summary>
    public async Task<Collection<Bucket>> ListBucketsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Listing all buckets");
            var buckets = await _client.ListBucketsAsync(cancellationToken);
            _logger.LogInformation("Successfully listed {Count} buckets", buckets.Buckets.Count);
            return buckets.Buckets;
        }
        catch (MinioException minioException)
        {
            _logger.LogError(minioException, "Minio exception while listing buckets");
            throw new MinioServiceException("Failed to list buckets", "list_buckets_failed", "An error occurred while listing buckets", minioException);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unexpected error while listing buckets");
            throw new MinioServiceException("Failed to list buckets", "list_buckets_failed", "An error occurred while listing buckets", exception);
        }
    }

    /// <summary>
    /// Creates a new bucket.
    /// </summary>
    public async Task MakeBucketAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(bucketName))
                throw new ArgumentException("Bucket name cannot be null or empty.", nameof(bucketName));

            _logger.LogInformation("Creating bucket: {BucketName}", bucketName);
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName), cancellationToken);
            _logger.LogInformation("Successfully created bucket: {BucketName}", bucketName);
        }
        catch (MinioException minioException)
        {
            _logger.LogError(minioException, "Minio exception while creating bucket: {BucketName}", bucketName);
            throw new BucketOperationException(bucketName, "create", minioException);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unexpected error while creating bucket: {BucketName}", bucketName);
            throw new BucketOperationException(bucketName, "create", exception);
        }
    }

    /// <summary>
    /// Checks if a bucket exists.
    /// </summary>
    public async Task<bool> BucketExistsAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(bucketName))
                throw new ArgumentException("Bucket name cannot be null or empty.", nameof(bucketName));

            _logger.LogDebug("Checking if bucket exists: {BucketName}", bucketName);
            var exists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName), cancellationToken);
            _logger.LogDebug("Bucket {BucketName} exists: {Exists}", bucketName, exists);
            return exists;
        }
        catch (MinioException minioException)
        {
            _logger.LogError(minioException, "Minio exception while checking bucket existence: {BucketName}", bucketName);
            throw new MinioServiceException($"Failed to check if bucket '{bucketName}' exists", "bucket_exists_failed", "An error occurred while checking bucket existence", minioException);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unexpected error while checking bucket existence: {BucketName}", bucketName);
            throw new MinioServiceException($"Failed to check if bucket '{bucketName}' exists", "bucket_exists_failed", "An error occurred while checking bucket existence", exception);
        }
    }

    /// <summary>
    /// Removes a bucket.
    /// </summary>
    public async Task RemoveBucketAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(bucketName))
                throw new ArgumentException("Bucket name cannot be null or empty.", nameof(bucketName));

            _logger.LogInformation("Removing bucket: {BucketName}", bucketName);
            await _client.RemoveBucketAsync(new RemoveBucketArgs().WithBucket(bucketName), cancellationToken);
            _logger.LogInformation("Successfully removed bucket: {BucketName}", bucketName);
        }
        catch (MinioException minioException)
        {
            _logger.LogError(minioException, "Minio exception while removing bucket: {BucketName}", bucketName);
            throw new BucketOperationException(bucketName, "remove", minioException);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unexpected error while removing bucket: {BucketName}", bucketName);
            throw new BucketOperationException(bucketName, "remove", exception);
        }
    }

    /// <summary>
    /// Puts an object into a bucket.
    /// </summary>
    public async Task PutObjectAsync(PutObjectRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrEmpty(request.BucketName))
                throw new ArgumentException("Bucket name cannot be null or empty.", nameof(request));
            if (string.IsNullOrEmpty(request.ObjectName))
                throw new ArgumentException("Object name cannot be null or empty.", nameof(request));
            if (request.Data == null)
                throw new ArgumentNullException(nameof(request.Data));

            _logger.LogInformation("Putting object: {ObjectName} to bucket: {BucketName}", request.ObjectName, request.BucketName);

            var args = new PutObjectArgs()
                .WithBucket(request.BucketName)
                .WithObject(request.ObjectName)
                .WithStreamData(request.Data)
                .WithObjectSize(request.Size);

            if (!string.IsNullOrEmpty(request.ContentType))
                args.WithContentType(request.ContentType);

            if (request.Metadata != null && request.Metadata.Count > 0)
                args.WithHeaders(request.Metadata);

            await _client.PutObjectAsync(args, cancellationToken);
            _logger.LogInformation("Successfully put object: {ObjectName} to bucket: {BucketName}", request.ObjectName, request.BucketName);
        }
        catch (MinioException minioException)
        {
            _logger.LogError(minioException, "Minio exception while putting object: {ObjectName} to bucket: {BucketName}", request?.ObjectName, request?.BucketName);
            throw new ObjectOperationException(request?.BucketName ?? "", request?.ObjectName ?? "", "put", minioException);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unexpected error while putting object: {ObjectName} to bucket: {BucketName}", request?.ObjectName, request?.BucketName);
            throw new ObjectOperationException(request?.BucketName ?? "", request?.ObjectName ?? "", "put", exception);
        }
    }

    /// <summary>
    /// Gets object metadata.
    /// </summary>
    public async Task<ObjectMetadata> StatObjectAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(bucketName))
                throw new ArgumentException("Bucket name cannot be null or empty.", nameof(bucketName));
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentException("Object name cannot be null or empty.", nameof(objectName));

            _logger.LogInformation("Getting metadata for object: {ObjectName} in bucket: {BucketName}", objectName, bucketName);

            var stat = await _client.StatObjectAsync(new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName), cancellationToken);

            var result = new ObjectMetadata
            {
                Name = objectName,
                Size = stat.Size,
                LastModified = stat.LastModified,
                ETag = stat.ETag,
                ContentType = stat.ContentType,
                VersionId = stat.VersionId
            };

            _logger.LogInformation("Successfully retrieved metadata for object: {ObjectName} in bucket: {BucketName}", objectName, bucketName);
            return result;
        }
        catch (MinioException minioException)
        {
            _logger.LogError(minioException, "Minio exception while getting metadata for object: {ObjectName} in bucket: {BucketName}", objectName, bucketName);
            throw new ObjectOperationException(bucketName, objectName, "get metadata", minioException);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unexpected error while getting metadata for object: {ObjectName} in bucket: {BucketName}", objectName, bucketName);
            throw new ObjectOperationException(bucketName, objectName, "get metadata", exception);
        }
    }

    /// <summary>
    /// Removes an object from a bucket.
    /// </summary>
    public async Task RemoveObjectAsync(string bucketName, string objectName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(bucketName))
                throw new ArgumentException("Bucket name cannot be null or empty.", nameof(bucketName));
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentException("Object name cannot be null or empty.", nameof(objectName));

            _logger.LogInformation("Removing object: {ObjectName} from bucket: {BucketName}", objectName, bucketName);

            await _client.RemoveObjectAsync(new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName), cancellationToken);

            _logger.LogInformation("Successfully removed object: {ObjectName} from bucket: {BucketName}", objectName, bucketName);
        }
        catch (MinioException minioException)
        {
            _logger.LogError(minioException, "Minio exception while removing object: {ObjectName} from bucket: {BucketName}", objectName, bucketName);
            throw new ObjectOperationException(bucketName, objectName, "remove", minioException);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unexpected error while removing object: {ObjectName} from bucket: {BucketName}", objectName, bucketName);
            throw new ObjectOperationException(bucketName, objectName, "remove", exception);
        }
    }

    /// <summary>
    /// Generates a presigned URL for getting an object.
    /// </summary>
    public async Task<PresignedUrlResult> PresignedGetObjectAsync(PresignedUrlRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrEmpty(request.BucketName)) throw new ArgumentException("Bucket name cannot be null or empty.", nameof(request));
            if (string.IsNullOrEmpty(request.ObjectName)) throw new ArgumentException("Object name cannot be null or empty.", nameof(request));

            _logger.LogInformation("Generating presigned GET URL for object: {ObjectName} in bucket: {BucketName}", request.ObjectName, request.BucketName);

            var args = new PresignedGetObjectArgs()
                .WithBucket(request.BucketName)
                .WithObject(request.ObjectName)
                .WithExpiry(request.ExpiresInSeconds);

            if (request.Headers != null && request.Headers.Count > 0)
                args.WithHeaders(request.Headers);

            var url = await _client.PresignedGetObjectAsync(args);

            var result = new PresignedUrlResult
            {
                Url = url,
                Expiration = DateTime.UtcNow.AddSeconds(request.ExpiresInSeconds),
                Method = "GET"
            };

            _logger.LogInformation("Successfully generated presigned GET URL for object: {ObjectName} in bucket: {BucketName}", request.ObjectName, request.BucketName);
            return result;
        }
        catch (MinioException minioException)
        {
            _logger.LogError(minioException, "Minio exception while generating presigned GET URL for object: {ObjectName} in bucket: {BucketName}", request?.ObjectName, request?.BucketName);
            throw new ObjectOperationException(request?.BucketName ?? "", request?.ObjectName ?? "", "generate presigned GET URL", minioException);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unexpected error while generating presigned GET URL for object: {ObjectName} in bucket: {BucketName}", request?.ObjectName, request?.BucketName);
            throw new ObjectOperationException(request?.BucketName ?? "", request?.ObjectName ?? "", "generate presigned GET URL", exception);
        }
    }

    /// <summary>
    /// Generates a presigned URL for putting an object.
    /// </summary>
    public async Task<PresignedUrlResult> PresignedPutObjectAsync(PresignedUrlRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrEmpty(request.BucketName)) throw new ArgumentException("Bucket name cannot be null or empty.", nameof(request));
            if (string.IsNullOrEmpty(request.ObjectName)) throw new ArgumentException("Object name cannot be null or empty.", nameof(request));

            _logger.LogInformation("Generating presigned PUT URL for object: {ObjectName} in bucket: {BucketName}", request.ObjectName, request.BucketName);

            var args = new PresignedPutObjectArgs()
                .WithBucket(request.BucketName)
                .WithObject(request.ObjectName)
                .WithExpiry(request.ExpiresInSeconds);

            if (request.Headers != null && request.Headers.Count > 0)
                args.WithHeaders(request.Headers);

            var url = await _client.PresignedPutObjectAsync(args);

            var result = new PresignedUrlResult
            {
                Url = url,
                Expiration = DateTime.UtcNow.AddSeconds(request.ExpiresInSeconds),
                Method = "PUT"
            };

            _logger.LogInformation("Successfully generated presigned PUT URL for object: {ObjectName} in bucket: {BucketName}", request.ObjectName, request.BucketName);
            return result;
        }
        catch (MinioException minioException)
        {
            _logger.LogError(minioException, "Minio exception while generating presigned PUT URL for object: {ObjectName} in bucket: {BucketName}", request?.ObjectName, request?.BucketName);
            throw new ObjectOperationException(request?.BucketName ?? "", request?.ObjectName ?? "", "generate presigned PUT URL", minioException);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unexpected error while generating presigned PUT URL for object: {ObjectName} in bucket: {BucketName}", request?.ObjectName, request?.BucketName);
            throw new ObjectOperationException(request?.BucketName ?? "", request?.ObjectName ?? "", "generate presigned PUT URL", exception);
        }
    }
}
