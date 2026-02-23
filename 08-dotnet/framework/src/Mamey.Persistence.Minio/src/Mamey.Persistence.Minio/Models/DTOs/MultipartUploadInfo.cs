namespace Mamey.Persistence.Minio.Models.DTOs;

/// <summary>
/// Information about a multipart upload operation.
/// </summary>
public class MultipartUploadInfo
{
    /// <summary>
    /// Gets or sets the upload ID.
    /// </summary>
    public string UploadId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bucket name.
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the object name.
    /// </summary>
    public string ObjectName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total size of the upload.
    /// </summary>
    public long TotalSize { get; set; }

    /// <summary>
    /// Gets or sets the part size used for the upload.
    /// </summary>
    public long PartSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of parts.
    /// </summary>
    public int TotalParts { get; set; }

    /// <summary>
    /// Gets or sets the number of completed parts.
    /// </summary>
    public int CompletedParts { get; set; }

    /// <summary>
    /// Gets or sets the upload status.
    /// </summary>
    public MultipartUploadStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the creation time.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last modified time.
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// Gets or sets the ETag of the completed upload.
    /// </summary>
    public string? ETag { get; set; }
}

/// <summary>
/// Status of a multipart upload operation.
/// </summary>
public enum MultipartUploadStatus
{
    /// <summary>
    /// Upload is in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// Upload has been completed.
    /// </summary>
    Completed,

    /// <summary>
    /// Upload has been aborted.
    /// </summary>
    Aborted,

    /// <summary>
    /// Upload has failed.
    /// </summary>
    Failed
}
