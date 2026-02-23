using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;

namespace Mamey.Persistence.Minio;

/// <summary>
/// Service for generating presigned URLs for Minio objects.
/// </summary>
public interface IPresignedUrlService
{
    /// <summary>
    /// Generates a presigned URL for getting an object.
    /// </summary>
    /// <param name="request">The presigned URL request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A presigned URL result.</returns>
    Task<PresignedUrlResult> PresignedGetObjectAsync(PresignedUrlRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a presigned URL for putting an object.
    /// </summary>
    /// <param name="request">The presigned URL request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A presigned URL result.</returns>
    Task<PresignedUrlResult> PresignedPutObjectAsync(PresignedUrlRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a presigned URL for posting an object (multipart upload).
    /// </summary>
    /// <param name="request">The presigned URL request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A presigned URL result.</returns>
    Task<PresignedUrlResult> PresignedPostObjectAsync(PresignedUrlRequest request, CancellationToken cancellationToken = default);
}
