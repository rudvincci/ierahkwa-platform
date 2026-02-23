namespace Mamey.Persistence.Minio.Models.Requests;

/// <summary>
/// Request for multipart upload operations.
/// </summary>
public class MultipartUploadRequest
{
    /// <summary>
    /// Gets or sets the bucket name.
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the object name.
    /// </summary>
    public string ObjectName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the stream data to upload.
    /// </summary>
    public Stream Stream { get; set; } = Stream.Null;

    /// <summary>
    /// Gets or sets the content type.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Gets or sets the metadata.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the part size in bytes. If not specified, will be calculated automatically.
    /// </summary>
    public long? PartSize { get; set; }

    /// <summary>
    /// Gets or sets the minimum part size in bytes.
    /// </summary>
    public long MinPartSize { get; set; } = 5 * 1024 * 1024; // 5MB

    /// <summary>
    /// Gets or sets the maximum part size in bytes.
    /// </summary>
    public long MaxPartSize { get; set; } = 5L * 1024 * 1024 * 1024; // 5GB

    /// <summary>
    /// Gets or sets the maximum number of concurrent part uploads.
    /// </summary>
    public int MaxConcurrency { get; set; } = 4;

    /// <summary>
    /// Gets or sets whether to resume an interrupted upload.
    /// </summary>
    public bool ResumeUpload { get; set; } = true;
}
