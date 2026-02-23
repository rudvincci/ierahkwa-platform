using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;

namespace Mamey.Persistence.Minio;

/// <summary>
/// Service for managing object lock and retention policies.
/// </summary>
public interface IObjectLockService
{
    /// <summary>
    /// Sets object lock configuration for a bucket.
    /// </summary>
    /// <param name="request">The set object lock configuration request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetObjectLockConfigurationAsync(SetObjectLockConfigurationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets object lock configuration for a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Object lock configuration.</returns>
    Task<ObjectLockConfiguration> GetObjectLockConfigurationAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets object retention.
    /// </summary>
    /// <param name="request">The set object retention request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetObjectRetentionAsync(SetObjectRetentionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets object retention.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object.</param>
    /// <param name="versionId">The version ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Object retention information.</returns>
    Task<ObjectRetention> GetObjectRetentionAsync(string bucketName, string objectName, string? versionId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets legal hold for an object.
    /// </summary>
    /// <param name="request">The set legal hold request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetLegalHoldAsync(SetLegalHoldRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets legal hold for an object.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object.</param>
    /// <param name="versionId">The version ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Legal hold status.</returns>
    Task<LegalHoldStatus> GetLegalHoldAsync(string bucketName, string objectName, string? versionId = null, CancellationToken cancellationToken = default);
}
