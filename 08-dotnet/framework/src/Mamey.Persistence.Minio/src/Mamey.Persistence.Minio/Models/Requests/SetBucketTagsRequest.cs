namespace Mamey.Persistence.Minio.Models.Requests;

/// <summary>
/// Request for setting tags on a bucket.
/// </summary>
public class SetBucketTagsRequest
{
    /// <summary>
    /// Gets or sets the bucket name.
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tags to set.
    /// </summary>
    public Dictionary<string, string> Tags { get; set; } = new();
}
