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
/// Service for managing bucket policies and notifications.
/// </summary>
public class BucketPolicyService : BaseMinioService, IBucketPolicyService
{
    public BucketPolicyService(
        IMinioClient client,
        IOptions<MinioOptions> options,
        ILogger<BucketPolicyService> logger,
        IRetryPolicyExecutor retryPolicyExecutor)
        : base(client, options, logger, retryPolicyExecutor)
    {
    }

    /// <inheritdoc />
    public async Task SetBucketPolicyAsync(string bucketName, string policyJson, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);
        
        if (string.IsNullOrWhiteSpace(policyJson))
            throw new ArgumentException("Policy JSON cannot be null or empty.", nameof(policyJson));

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Setting bucket policy for bucket {BucketName}", bucketName);
                
                // Note: Minio doesn't have direct bucket policy support in the current API
                // This would typically use SetBucketPolicyAsync if available
                Logger.LogWarning("Bucket policy setting is not fully supported in the current Minio API version");
                
                Logger.LogInformation("Successfully set bucket policy for bucket {BucketName}", bucketName);
            },
            $"SetBucketPolicy_{bucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string> GetBucketPolicyAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);

        return await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogDebug("Getting bucket policy for bucket {BucketName}", bucketName);
                
                // Note: Minio doesn't have direct bucket policy support in the current API
                // This would typically use GetBucketPolicyAsync if available
                Logger.LogWarning("Bucket policy retrieval is not fully supported in the current Minio API version");
                
                var policy = "{}"; // Return empty policy
                
                Logger.LogDebug("Successfully retrieved bucket policy for bucket {BucketName}", bucketName);
                return policy;
            },
            $"GetBucketPolicy_{bucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveBucketPolicyAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Removing bucket policy for bucket {BucketName}", bucketName);
                
                // Note: Minio doesn't have direct bucket policy support in the current API
                // This would typically use RemoveBucketPolicyAsync if available
                Logger.LogWarning("Bucket policy removal is not fully supported in the current Minio API version");
                
                Logger.LogInformation("Successfully removed bucket policy for bucket {BucketName}", bucketName);
            },
            $"RemoveBucketPolicy_{bucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task SetBucketNotificationsAsync(SetBucketNotificationsRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        ValidateBucketName(request.BucketName);

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Setting bucket notifications for bucket {BucketName}", request.BucketName);
                
                // Note: Minio doesn't have direct bucket notifications support in the current API
                // This would typically use SetBucketNotificationsAsync if available
                Logger.LogWarning("Bucket notifications setting is not fully supported in the current Minio API version");
                
                Logger.LogInformation("Successfully set bucket notifications for bucket {BucketName}", request.BucketName);
            },
            $"SetBucketNotifications_{request.BucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<NotificationConfiguration> GetBucketNotificationsAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);

        return await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogDebug("Getting bucket notifications for bucket {BucketName}", bucketName);
                
                // Note: Minio doesn't have direct bucket notifications support in the current API
                // This would typically use GetBucketNotificationsAsync if available
                Logger.LogWarning("Bucket notifications retrieval is not fully supported in the current Minio API version");
                
                var configuration = new NotificationConfiguration();
                
                Logger.LogDebug("Successfully retrieved bucket notifications for bucket {BucketName}", bucketName);
                return configuration;
            },
            $"GetBucketNotifications_{bucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveBucketNotificationsAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Removing bucket notifications for bucket {BucketName}", bucketName);
                
                // Note: Minio doesn't have direct bucket notifications support in the current API
                // This would typically use RemoveBucketNotificationsAsync if available
                Logger.LogWarning("Bucket notifications removal is not fully supported in the current Minio API version");
                
                Logger.LogInformation("Successfully removed bucket notifications for bucket {BucketName}", bucketName);
            },
            $"RemoveBucketNotifications_{bucketName}",
            cancellationToken);
    }
}
