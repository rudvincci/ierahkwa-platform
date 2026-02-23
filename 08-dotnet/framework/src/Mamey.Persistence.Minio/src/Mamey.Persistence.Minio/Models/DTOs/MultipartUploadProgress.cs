namespace Mamey.Persistence.Minio.Models.DTOs;

/// <summary>
/// Progress information for multipart upload operations.
/// </summary>
public class MultipartUploadProgress
{
    /// <summary>
    /// Gets or sets the upload ID.
    /// </summary>
    public string UploadId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total size of the upload.
    /// </summary>
    public long TotalSize { get; set; }

    /// <summary>
    /// Gets or sets the number of bytes uploaded so far.
    /// </summary>
    public long BytesUploaded { get; set; }

    /// <summary>
    /// Gets or sets the percentage of completion (0-100).
    /// </summary>
    public double Percentage => TotalSize > 0 ? (double)BytesUploaded / TotalSize * 100 : 0;

    /// <summary>
    /// Gets or sets the total number of parts.
    /// </summary>
    public int TotalParts { get; set; }

    /// <summary>
    /// Gets or sets the number of completed parts.
    /// </summary>
    public int CompletedParts { get; set; }

    /// <summary>
    /// Gets or sets the number of parts currently being uploaded.
    /// </summary>
    public int PartsInProgress { get; set; }

    /// <summary>
    /// Gets or sets the upload speed in bytes per second.
    /// </summary>
    public long BytesPerSecond { get; set; }

    /// <summary>
    /// Gets or sets the estimated time remaining.
    /// </summary>
    public TimeSpan? EstimatedTimeRemaining { get; set; }

    /// <summary>
    /// Gets or sets the current part being uploaded.
    /// </summary>
    public int CurrentPart { get; set; }

    /// <summary>
    /// Gets or sets the size of the current part.
    /// </summary>
    public long CurrentPartSize { get; set; }
}
