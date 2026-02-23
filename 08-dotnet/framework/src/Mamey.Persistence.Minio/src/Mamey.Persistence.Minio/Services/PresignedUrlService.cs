using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Mamey.Persistence.Minio.Exceptions;
using Mamey.Persistence.Minio.Infrastructure;
using Mamey.Persistence.Minio.Infrastructure.Resilience;
using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;

namespace Mamey.Persistence.Minio.Services;

/// <summary>
/// Service for generating presigned URLs for Minio objects.
/// </summary>
public class PresignedUrlService : BaseMinioService, IPresignedUrlService
{
    public PresignedUrlService(
        IMinioClient client,
        IOptions<MinioOptions> options,
        ILogger<PresignedUrlService> logger,
        IRetryPolicyExecutor retryPolicyExecutor)
        : base(client, options, logger, retryPolicyExecutor)
    {
    }

    /// <inheritdoc />
    public async Task<PresignedUrlResult> PresignedGetObjectAsync(PresignedUrlRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        ValidateBucketAndObjectNames(request.BucketName, request.ObjectName);

        return await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogDebug("Generating presigned GET URL for object {ObjectName} in bucket {BucketName}", request.ObjectName, request.BucketName);
                
                var args = new PresignedGetObjectArgs()
                    .WithBucket(request.BucketName)
                    .WithObject(request.ObjectName)
                    .WithExpiry(request.ExpiresInSeconds);

                if (request.Headers != null && request.Headers.Count > 0)
                    args.WithHeaders(request.Headers);

                var url = await Client.PresignedGetObjectAsync(args);
                
                var result = new PresignedUrlResult
                {
                    Url = url,
                    Expiration = DateTime.UtcNow.AddSeconds(request.ExpiresInSeconds),
                    Method = "GET",
                    BucketName = request.BucketName,
                    ObjectName = request.ObjectName
                };

                Logger.LogDebug("Successfully generated presigned GET URL for object {ObjectName} in bucket {BucketName}", request.ObjectName, request.BucketName);
                return result;
            },
            $"PresignedGetObject_{request.BucketName}_{request.ObjectName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PresignedUrlResult> PresignedPutObjectAsync(PresignedUrlRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        ValidateBucketAndObjectNames(request.BucketName, request.ObjectName);

        return await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogDebug("Generating presigned PUT URL for object {ObjectName} in bucket {BucketName}", request.ObjectName, request.BucketName);
                
                var args = new PresignedPutObjectArgs()
                    .WithBucket(request.BucketName)
                    .WithObject(request.ObjectName)
                    .WithExpiry(request.ExpiresInSeconds);

                if (request.Headers != null && request.Headers.Count > 0)
                    args.WithHeaders(request.Headers);

                var url = await Client.PresignedPutObjectAsync(args);
                
                var result = new PresignedUrlResult
                {
                    Url = url,
                    Expiration = DateTime.UtcNow.AddSeconds(request.ExpiresInSeconds),
                    Method = "PUT",
                    BucketName = request.BucketName,
                    ObjectName = request.ObjectName
                };

                Logger.LogDebug("Successfully generated presigned PUT URL for object {ObjectName} in bucket {BucketName}", request.ObjectName, request.BucketName);
                return result;
            },
            $"PresignedPutObject_{request.BucketName}_{request.ObjectName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PresignedUrlResult> PresignedPostObjectAsync(PresignedUrlRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        ValidateBucketAndObjectNames(request.BucketName, request.ObjectName);

        // Note: Presigned POST is not available in the current Minio API version
        // For now, we'll return a presigned PUT URL as an alternative
        Logger.LogWarning("Presigned POST is not available in the current Minio API version. Using presigned PUT as alternative.");
        return await PresignedPutObjectAsync(request, cancellationToken);
    }
}
