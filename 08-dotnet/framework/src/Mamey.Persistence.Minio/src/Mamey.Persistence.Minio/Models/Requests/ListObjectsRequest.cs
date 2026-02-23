namespace Mamey.Persistence.Minio.Models.Requests;

/// <summary>
/// Represents a request to list objects.
/// </summary>
public class ListObjectsRequest
{
    /// <summary>
    /// Gets or sets the bucket name.
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the prefix to filter objects.
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// Gets or sets whether to include versions.
    /// </summary>
    public bool IncludeVersions { get; set; }

    /// <summary>
    /// Gets or sets whether to include delete markers.
    /// </summary>
    public bool IncludeDeleteMarkers { get; set; }

    /// <summary>
    /// Gets or sets the delimiter for grouping.
    /// </summary>
    public string? Delimiter { get; set; }

    /// <summary>
    /// Gets or sets the marker for pagination.
    /// </summary>
    public string? Marker { get; set; }

    /// <summary>
    /// Gets or sets the version ID marker for pagination.
    /// </summary>
    public string? VersionIdMarker { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of objects to return.
    /// </summary>
    public int? MaxKeys { get; set; }

    /// <summary>
    /// Gets or sets whether to list recursively.
    /// </summary>
    public bool? Recursive { get; set; }
}
