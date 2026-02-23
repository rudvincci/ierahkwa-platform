using System;

namespace Mamey.Persistence.Minio.Models.DTOs;

/// <summary>
/// Represents information about an object in Minio.
/// </summary>
public class ObjectInfo
{
    /// <summary>
    /// Gets or sets the name of the object.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the size of the object in bytes.
    /// </summary>
    public ulong Size { get; set; }

    /// <summary>
    /// Gets or sets the last modified date of the object.
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// Gets or sets the ETag of the object.
    /// </summary>
    public string ETag { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the object is a directory.
    /// </summary>
    public bool IsDir { get; set; }

    /// <summary>
    /// Gets or sets the version ID of the object.
    /// </summary>
    public string? VersionId { get; set; }

    /// <summary>
    /// Gets or sets whether the object is a delete marker.
    /// </summary>
    public bool IsDeleteMarker { get; set; }
}
