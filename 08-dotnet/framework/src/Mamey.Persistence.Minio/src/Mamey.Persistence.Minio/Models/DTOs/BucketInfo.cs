using System;

namespace Mamey.Persistence.Minio.Models.DTOs;

/// <summary>
/// Represents information about a Minio bucket.
/// </summary>
public class BucketInfo
{
    /// <summary>
    /// Gets or sets the name of the bucket.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation date of the bucket.
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Gets or sets the region where the bucket is located.
    /// </summary>
    public string? Region { get; set; }
}
