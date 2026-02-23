using System.Collections.ObjectModel;
using Minio.DataModel;
using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;

namespace Mamey.Persistence.Minio;

/// <summary>
/// Service for managing Minio buckets.
/// </summary>
public interface IBucketService
{
    /// <summary>
    /// Lists all buckets.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of buckets.</returns>
    Task<Collection<global::Minio.DataModel.Bucket>> ListBucketsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket to create.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task MakeBucketAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a bucket exists.
    /// </summary>
    /// <param name="bucketName">The name of the bucket to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the bucket exists, false otherwise.</returns>
    Task<bool> BucketExistsAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket to remove.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveBucketAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists objects in a bucket.
    /// </summary>
    /// <param name="request">The list objects request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of object information.</returns>
    Task<Collection<ObjectInfo>> ListObjectsAsync(ListObjectsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables versioning on a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task EnableVersioningAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables versioning on a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DisableVersioningAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets versioning information for a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Versioning information.</returns>
    Task<BucketVersioningInfo> GetVersioningAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the versioning status for a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="status">The versioning status to set.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetBucketVersioningAsync(string bucketName, VersioningStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets versioning information for a bucket (alias for GetVersioningAsync).
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Versioning information.</returns>
    Task<BucketVersioningInfo> GetBucketVersioningAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets tags on a bucket.
    /// </summary>
    /// <param name="request">The set bucket tags request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetBucketTagsAsync(SetBucketTagsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets tags on a bucket (convenience overload).
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="tags">The tags to set.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetBucketTagsAsync(string bucketName, Dictionary<string, string> tags, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets tags from a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Bucket tags.</returns>
    Task<Tags> GetBucketTagsAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all tags from a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveBucketTagsAsync(string bucketName, CancellationToken cancellationToken = default);
}
