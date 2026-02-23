using System;
using System.Collections.Generic;

namespace Mamey.Persistence.Minio.Models.DTOs;

/// <summary>
/// Represents metadata for an object in Minio.
/// </summary>
public class ObjectMetadata
{
    /// <summary>
    /// Gets or sets the name of the object.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size of the object in bytes.
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Gets or sets the last modified date of the object.
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// Gets or sets the ETag of the object.
    /// </summary>
    public string ETag { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content type of the object.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Gets or sets the version ID of the object.
    /// </summary>
    public string? VersionId { get; set; }

    /// <summary>
    /// Gets or sets whether the object is a delete marker.
    /// </summary>
    public bool IsDeleteMarker { get; set; }

    /// <summary>
    /// Gets or sets the storage class of the object.
    /// </summary>
    public string? StorageClass { get; set; }

    /// <summary>
    /// Gets or sets the user metadata.
    /// </summary>
    public Dictionary<string, string> UserMetadata { get; set; } = new();

    /// <summary>
    /// Gets or sets the system metadata.
    /// </summary>
    public Dictionary<string, string> SystemMetadata { get; set; } = new();
}
