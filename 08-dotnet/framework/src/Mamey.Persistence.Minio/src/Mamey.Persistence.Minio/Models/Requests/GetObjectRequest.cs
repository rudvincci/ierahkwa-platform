namespace Mamey.Persistence.Minio.Models.Requests;

/// <summary>
/// Represents a request to get an object.
/// </summary>
public class GetObjectRequest
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
    /// Gets or sets the version ID.
    /// </summary>
    public string? VersionId { get; set; }

    /// <summary>
    /// Gets or sets the server-side encryption headers.
    /// </summary>
    public Dictionary<string, string>? ServerSideEncryptionHeaders { get; set; }

    /// <summary>
    /// Gets or sets the offset to start reading from.
    /// </summary>
    public long? Offset { get; set; }

    /// <summary>
    /// Gets or sets the length to read.
    /// </summary>
    public long? Length { get; set; }
}
