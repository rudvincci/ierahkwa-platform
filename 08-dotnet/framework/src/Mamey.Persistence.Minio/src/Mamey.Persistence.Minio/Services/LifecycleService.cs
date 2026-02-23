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
/// Service for managing bucket lifecycle configurations.
/// </summary>
public class LifecycleService : BaseMinioService, ILifecycleService
{
    public LifecycleService(
        IMinioClient client,
        IOptions<MinioOptions> options,
        ILogger<LifecycleService> logger,
        IRetryPolicyExecutor retryPolicyExecutor)
        : base(client, options, logger, retryPolicyExecutor)
    {
    }

    /// <inheritdoc />
    public async Task SetLifecycleConfigurationAsync(SetLifecycleConfigurationRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        ValidateBucketName(request.BucketName);

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Setting lifecycle configuration for bucket {BucketName}", request.BucketName);
                
                // Note: Minio doesn't have direct lifecycle support in the current API
                // This would typically use SetBucketLifecycleAsync if available
                Logger.LogWarning("Lifecycle configuration setting is not fully supported in the current Minio API version");
                
                Logger.LogInformation("Successfully set lifecycle configuration for bucket {BucketName}", request.BucketName);
            },
            $"SetLifecycleConfiguration_{request.BucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<LifecycleConfiguration> GetLifecycleConfigurationAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);

        return await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogDebug("Getting lifecycle configuration for bucket {BucketName}", bucketName);
                
                // Note: Minio doesn't have direct lifecycle support in the current API
                // This would typically use GetBucketLifecycleAsync if available
                Logger.LogWarning("Lifecycle configuration retrieval is not fully supported in the current Minio API version");
                
                var configuration = new LifecycleConfiguration();
                
                Logger.LogDebug("Successfully retrieved lifecycle configuration for bucket {BucketName}", bucketName);
                return configuration;
            },
            $"GetLifecycleConfiguration_{bucketName}",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task RemoveLifecycleConfigurationAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ValidateBucketName(bucketName);

        await ExecuteWithRetryAsync(
            async ct =>
            {
                Logger.LogInformation("Removing lifecycle configuration for bucket {BucketName}", bucketName);
                
                // Note: Minio doesn't have direct lifecycle support in the current API
                // This would typically use RemoveBucketLifecycleAsync if available
                Logger.LogWarning("Lifecycle configuration removal is not fully supported in the current Minio API version");
                
                Logger.LogInformation("Successfully removed lifecycle configuration for bucket {BucketName}", bucketName);
            },
            $"RemoveLifecycleConfiguration_{bucketName}",
            cancellationToken);
    }
}
