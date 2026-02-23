using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Mamey.Biometrics.Storage.MinIO;

/// <summary>
/// Interface for storing biometric images in MinIO.
/// </summary>
public interface IBiometricImageStore
{
    /// <summary>
    /// Upload an image to MinIO.
    /// </summary>
    /// <param name="templateId">Template ID</param>
    /// <param name="imageStream">Image stream</param>
    /// <param name="contentType">Content type (e.g., image/jpeg)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Object ID for the uploaded image</returns>
    Task<string> UploadImageAsync(Guid templateId, Stream imageStream, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Download an image from MinIO.
    /// </summary>
    /// <param name="objectId">Object ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Image stream</returns>
    Task<Stream> DownloadImageAsync(string objectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an image from MinIO.
    /// </summary>
    /// <param name="objectId">Object ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteImageAsync(string objectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if an image exists in MinIO.
    /// </summary>
    /// <param name="objectId">Object ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if exists, false otherwise</returns>
    Task<bool> ImageExistsAsync(string objectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get image metadata.
    /// </summary>
    /// <param name="objectId">Object ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Image metadata or null if not found</returns>
    Task<ImageMetadata?> GetImageMetadataAsync(string objectId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ensure the bucket exists.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if bucket exists or was created</returns>
    Task<bool> EnsureBucketExistsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Image metadata.
/// </summary>
public class ImageMetadata
{
    /// <summary>
    /// Object ID
    /// </summary>
    public string ObjectId { get; set; } = string.Empty;

    /// <summary>
    /// Content type
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Content length in bytes
    /// </summary>
    public long ContentLength { get; set; }

    /// <summary>
    /// Last modified date
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// ETag
    /// </summary>
    public string ETag { get; set; } = string.Empty;
}
