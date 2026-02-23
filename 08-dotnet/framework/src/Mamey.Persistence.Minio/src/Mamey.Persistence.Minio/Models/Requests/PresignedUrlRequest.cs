namespace Mamey.Persistence.Minio.Models.Requests;

/// <summary>
/// Request for generating presigned URLs.
/// </summary>
public class PresignedUrlRequest
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
    /// Gets or sets the expiration time in seconds.
    /// </summary>
    public int ExpiresInSeconds { get; set; } = 3600;

    /// <summary>
    /// Gets or sets additional headers.
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }
}