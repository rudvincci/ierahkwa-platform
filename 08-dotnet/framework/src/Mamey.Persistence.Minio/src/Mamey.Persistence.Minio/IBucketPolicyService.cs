using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;

namespace Mamey.Persistence.Minio;

/// <summary>
/// Service for managing bucket policies and notifications.
/// </summary>
public interface IBucketPolicyService
{
    /// <summary>
    /// Sets a bucket policy.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="policyJson">The policy JSON string.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetBucketPolicyAsync(string bucketName, string policyJson, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the bucket policy.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The bucket policy JSON string.</returns>
    Task<string> GetBucketPolicyAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the bucket policy.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveBucketPolicyAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets bucket notifications.
    /// </summary>
    /// <param name="request">The set bucket notifications request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetBucketNotificationsAsync(SetBucketNotificationsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets bucket notifications.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Bucket notifications configuration.</returns>
    Task<NotificationConfiguration> GetBucketNotificationsAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes bucket notifications.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveBucketNotificationsAsync(string bucketName, CancellationToken cancellationToken = default);
}
