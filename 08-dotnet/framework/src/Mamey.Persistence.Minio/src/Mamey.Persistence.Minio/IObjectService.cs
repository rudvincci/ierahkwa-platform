using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;
using Mamey.Persistence.Minio.Builders;

namespace Mamey.Persistence.Minio;

/// <summary>
/// Service for managing Minio objects.
/// </summary>
public interface IObjectService
{
    /// <summary>
    /// Puts an object into a bucket.
    /// </summary>
    /// <param name="request">The put object request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task PutObjectAsync(PutObjectRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets object metadata.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Object metadata.</returns>
    Task<ObjectMetadata> StatObjectAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an object from a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveObjectAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Copies an object to another location.
    /// </summary>
    /// <param name="request">The copy object request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CopyObjectAsync(CopyObjectRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a file to a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object.</param>
    /// <param name="filePath">The path to the file to upload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Object metadata.</returns>
    Task<ObjectMetadata> UploadFileAsync(string bucketName, string objectName, string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads bytes to a bucket.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object.</param>
    /// <param name="data">The data to upload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Object metadata.</returns>
    Task<ObjectMetadata> UploadBytesAsync(string bucketName, string objectName, byte[] data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads an object to a file.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object.</param>
    /// <param name="filePath">The path where to save the file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DownloadToFileAsync(string bucketName, string objectName, string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads an object as bytes.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The object data as bytes.</returns>
    Task<byte[]> DownloadAsBytesAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads an object to a stream with progress reporting.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object.</param>
    /// <param name="destination">The destination stream.</param>
    /// <param name="progress">The progress reporter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DownloadAsync(string bucketName, string objectName, Stream destination, IProgress<long>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a large file using multipart upload.
    /// </summary>
    /// <param name="request">The multipart upload request.</param>
    /// <param name="progress">The progress reporter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Object metadata.</returns>
    Task<ObjectMetadata> UploadMultipartAsync(MultipartUploadRequest request, IProgress<MultipartUploadProgress>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads an object using the fluent builder pattern.
    /// </summary>
    /// <param name="request">The put object request.</param>
    /// <param name="progress">The progress reporter for multipart uploads.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Object metadata.</returns>
    Task<ObjectMetadata> UploadAsync(PutObjectRequest request, IProgress<MultipartUploadProgress>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads an object using the fluent builder pattern.
    /// </summary>
    /// <param name="configuration">The download configuration.</param>
    /// <param name="destination">The destination stream.</param>
    /// <param name="progress">The progress reporter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DownloadAsync(ObjectDownloadConfiguration configuration, Stream destination, IProgress<long>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a file from the local filesystem.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object.</param>
    /// <param name="filePath">The path to the file to upload.</param>
    /// <param name="contentType">The content type. If null, will be determined from file extension.</param>
    /// <param name="metadata">The metadata to associate with the object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Object metadata.</returns>
    Task<ObjectMetadata> UploadFileAsync(string bucketName, string objectName, string filePath, string? contentType = null, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads an object to the local filesystem.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object.</param>
    /// <param name="filePath">The path where to save the file.</param>
    /// <param name="progress">The progress reporter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DownloadToFileAsync(string bucketName, string objectName, string filePath, IProgress<long>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads data from a byte array.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object.</param>
    /// <param name="data">The data to upload.</param>
    /// <param name="contentType">The content type.</param>
    /// <param name="metadata">The metadata to associate with the object.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Object metadata.</returns>
    Task<ObjectMetadata> UploadBytesAsync(string bucketName, string objectName, byte[] data, string? contentType = null, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads an object as a byte array.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object.</param>
    /// <param name="progress">The progress reporter.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The object data as bytes.</returns>
    Task<byte[]> DownloadAsBytesAsync(string bucketName, string objectName, IProgress<long>? progress = null, CancellationToken cancellationToken = default);
}
