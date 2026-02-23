namespace Mamey.Persistence.Minio.Models.Requests;

/// <summary>
/// Request for copying an object.
/// </summary>
public class CopyObjectRequest
{
    /// <summary>
    /// Gets or sets the source bucket name.
    /// </summary>
    public string SourceBucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source object name.
    /// </summary>
    public string SourceObjectName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the destination bucket name.
    /// </summary>
    public string DestinationBucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the destination object name.
    /// </summary>
    public string DestinationObjectName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content type.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Gets or sets the headers.
    /// </summary>
    public Dictionary<string, string>? Headers { get; set; }
}