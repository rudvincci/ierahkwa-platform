namespace Mamey.Persistence.Minio.Models.Requests;

/// <summary>
/// Request for putting an object into a bucket.
/// </summary>
public class PutObjectRequest
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
    /// Gets or sets the data stream.
    /// </summary>
    public Stream Data { get; set; } = null!;

    /// <summary>
    /// Gets or sets the object size.
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Gets or sets the content type.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Gets or sets the metadata.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
}