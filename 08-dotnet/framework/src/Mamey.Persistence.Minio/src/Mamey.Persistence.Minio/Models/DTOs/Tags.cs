using System.Collections.Generic;

namespace Mamey.Persistence.Minio.Models.DTOs;

/// <summary>
/// Represents a collection of tags.
/// </summary>
public class Tags
{
    /// <summary>
    /// Gets or sets the tag set.
    /// </summary>
    public List<TagSet> TagSet { get; set; } = new();
}

/// <summary>
/// Represents a tag key-value pair.
/// </summary>
public class TagSet
{
    /// <summary>
    /// Gets or sets the tag key.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tag value.
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
