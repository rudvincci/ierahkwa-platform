using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;

namespace Mamey.Persistence.Minio.Builders;

/// <summary>
/// Fluent builder for configuring object upload operations.
/// </summary>
public class ObjectUploadBuilder
{
    private readonly string _bucketName;
    private readonly string _objectName;
    private readonly Stream _stream;
    private readonly Dictionary<string, string> _metadata = new();
    private readonly Dictionary<string, string> _headers = new();
    private string? _contentType;
    private bool _useMultipart = false;
    private long _partSize = 5 * 1024 * 1024; // 5MB default
    private int _maxConcurrency = 4;
    private bool _resumeUpload = true;

    internal ObjectUploadBuilder(string bucketName, string objectName, Stream stream)
    {
        _bucketName = bucketName;
        _objectName = objectName;
        _stream = stream;
    }

    /// <summary>
    /// Sets the content type for the upload.
    /// </summary>
    /// <param name="contentType">The content type.</param>
    /// <returns>The builder instance.</returns>
    public ObjectUploadBuilder WithContentType(string contentType)
    {
        _contentType = contentType;
        return this;
    }

    /// <summary>
    /// Sets the content type based on file extension.
    /// </summary>
    /// <param name="fileName">The file name to determine content type.</param>
    /// <returns>The builder instance.</returns>
    public ObjectUploadBuilder WithContentTypeFromFile(string fileName)
    {
        _contentType = GetContentTypeFromFileName(fileName);
        return this;
    }

    /// <summary>
    /// Adds metadata to the upload.
    /// </summary>
    /// <param name="key">The metadata key.</param>
    /// <param name="value">The metadata value.</param>
    /// <returns>The builder instance.</returns>
    public ObjectUploadBuilder WithMetadata(string key, string value)
    {
        _metadata[key] = value;
        return this;
    }

    /// <summary>
    /// Adds multiple metadata entries to the upload.
    /// </summary>
    /// <param name="metadata">The metadata dictionary.</param>
    /// <returns>The builder instance.</returns>
    public ObjectUploadBuilder WithMetadata(Dictionary<string, string> metadata)
    {
        foreach (var kvp in metadata)
        {
            _metadata[kvp.Key] = kvp.Value;
        }
        return this;
    }

    /// <summary>
    /// Adds a header to the upload.
    /// </summary>
    /// <param name="key">The header key.</param>
    /// <param name="value">The header value.</param>
    /// <returns>The builder instance.</returns>
    public ObjectUploadBuilder WithHeader(string key, string value)
    {
        _headers[key] = value;
        return this;
    }

    /// <summary>
    /// Adds multiple headers to the upload.
    /// </summary>
    /// <param name="headers">The headers dictionary.</param>
    /// <returns>The builder instance.</returns>
    public ObjectUploadBuilder WithHeaders(Dictionary<string, string> headers)
    {
        foreach (var kvp in headers)
        {
            _headers[kvp.Key] = kvp.Value;
        }
        return this;
    }

    /// <summary>
    /// Enables multipart upload for large files.
    /// </summary>
    /// <param name="partSize">The part size in bytes. If not specified, will be calculated automatically.</param>
    /// <param name="maxConcurrency">The maximum number of concurrent part uploads.</param>
    /// <param name="resumeUpload">Whether to resume interrupted uploads.</param>
    /// <returns>The builder instance.</returns>
    public ObjectUploadBuilder WithMultipartUpload(long? partSize = null, int? maxConcurrency = null, bool? resumeUpload = null)
    {
        _useMultipart = true;
        if (partSize.HasValue)
            _partSize = partSize.Value;
        if (maxConcurrency.HasValue)
            _maxConcurrency = maxConcurrency.Value;
        if (resumeUpload.HasValue)
            _resumeUpload = resumeUpload.Value;
        return this;
    }

    /// <summary>
    /// Enables server-side encryption.
    /// </summary>
    /// <param name="algorithm">The encryption algorithm.</param>
    /// <param name="keyId">The key ID for KMS encryption.</param>
    /// <returns>The builder instance.</returns>
    public ObjectUploadBuilder WithEncryption(string algorithm = "AES256", string? keyId = null)
    {
        _headers["x-amz-server-side-encryption"] = algorithm;
        if (!string.IsNullOrEmpty(keyId))
        {
            _headers["x-amz-server-side-encryption-aws-kms-key-id"] = keyId;
        }
        return this;
    }

    /// <summary>
    /// Sets object tags for the upload.
    /// </summary>
    /// <param name="tags">The tags dictionary.</param>
    /// <returns>The builder instance.</returns>
    public ObjectUploadBuilder WithTags(Dictionary<string, string> tags)
    {
        var tagString = string.Join("&", tags.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
        _headers["x-amz-tagging"] = tagString;
        return this;
    }

    /// <summary>
    /// Sets cache control for the upload.
    /// </summary>
    /// <param name="cacheControl">The cache control value.</param>
    /// <returns>The builder instance.</returns>
    public ObjectUploadBuilder WithCacheControl(string cacheControl)
    {
        _headers["Cache-Control"] = cacheControl;
        return this;
    }

    /// <summary>
    /// Sets content disposition for the upload.
    /// </summary>
    /// <param name="contentDisposition">The content disposition value.</param>
    /// <returns>The builder instance.</returns>
    public ObjectUploadBuilder WithContentDisposition(string contentDisposition)
    {
        _headers["Content-Disposition"] = contentDisposition;
        return this;
    }

    /// <summary>
    /// Builds the upload request.
    /// </summary>
    /// <returns>The configured upload request.</returns>
    public PutObjectRequest Build()
    {
        return new PutObjectRequest
        {
            BucketName = _bucketName,
            ObjectName = _objectName,
            Data = _stream,
            Size = _stream.Length,
            ContentType = _contentType,
            Metadata = _metadata.Count > 0 ? _metadata : null
        };
    }

    /// <summary>
    /// Builds the multipart upload request.
    /// </summary>
    /// <returns>The configured multipart upload request.</returns>
    public MultipartUploadRequest BuildMultipart()
    {
        return new MultipartUploadRequest
        {
            BucketName = _bucketName,
            ObjectName = _objectName,
            Stream = _stream,
            ContentType = _contentType,
            Metadata = _metadata.Count > 0 ? _metadata : null,
            PartSize = _partSize,
            MaxConcurrency = _maxConcurrency,
            ResumeUpload = _resumeUpload
        };
    }

    /// <summary>
    /// Determines if multipart upload should be used.
    /// </summary>
    /// <returns>True if multipart upload should be used.</returns>
    public bool ShouldUseMultipart()
    {
        return _useMultipart || _stream.Length > 100 * 1024 * 1024; // 100MB threshold
    }

    private static string GetContentTypeFromFileName(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".txt" => "text/plain",
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".pdf" => "application/pdf",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".zip" => "application/zip",
            ".mp4" => "video/mp4",
            ".mp3" => "audio/mpeg",
            _ => "application/octet-stream"
        };
    }
}
