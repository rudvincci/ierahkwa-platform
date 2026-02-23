using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;

namespace Mamey.Persistence.Minio;

/// <summary>
/// Service for managing bucket lifecycle configurations.
/// </summary>
public interface ILifecycleService
{
    /// <summary>
    /// Sets lifecycle configuration for a bucket.
    /// </summary>
    /// <param name="request">The set lifecycle request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetLifecycleConfigurationAsync(SetLifecycleConfigurationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets lifecycle configuration for a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Lifecycle configuration.</returns>
    Task<LifecycleConfiguration> GetLifecycleConfigurationAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes lifecycle configuration from a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveLifecycleConfigurationAsync(string bucketName, CancellationToken cancellationToken = default);
}
