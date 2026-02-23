namespace Mamey.Persistence.Minio.Models.DTOs;

/// <summary>
/// Represents a lifecycle configuration for a bucket.
/// </summary>
public class LifecycleConfiguration
{
    /// <summary>
    /// Gets or sets the lifecycle rules.
    /// </summary>
    public List<LifecycleRule> Rules { get; set; } = new();
}

/// <summary>
/// Represents a lifecycle rule.
/// </summary>
public class LifecycleRule
{
    /// <summary>
    /// Gets or sets the rule ID.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rule status.
    /// </summary>
    public LifecycleRuleStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the filter.
    /// </summary>
    public LifecycleFilter Filter { get; set; } = new();

    /// <summary>
    /// Gets or sets the transitions.
    /// </summary>
    public List<LifecycleTransition> Transitions { get; set; } = new();

    /// <summary>
    /// Gets or sets the expiration.
    /// </summary>
    public LifecycleExpiration? Expiration { get; set; }

    /// <summary>
    /// Gets or sets the noncurrent version expiration.
    /// </summary>
    public LifecycleNoncurrentVersionExpiration? NoncurrentVersionExpiration { get; set; }

    /// <summary>
    /// Gets or sets the abort incomplete multipart upload.
    /// </summary>
    public LifecycleAbortIncompleteMultipartUpload? AbortIncompleteMultipartUpload { get; set; }
}

/// <summary>
/// Represents the status of a lifecycle rule.
/// </summary>
public enum LifecycleRuleStatus
{
    /// <summary>
    /// Rule is enabled.
    /// </summary>
    Enabled,

    /// <summary>
    /// Rule is disabled.
    /// </summary>
    Disabled
}

/// <summary>
/// Represents a lifecycle filter.
/// </summary>
public class LifecycleFilter
{
    /// <summary>
    /// Gets or sets the prefix.
    /// </summary>
    public string? Prefix { get; set; }

    /// <summary>
    /// Gets or sets the tags.
    /// </summary>
    public List<LifecycleTag> Tags { get; set; } = new();
}

/// <summary>
/// Represents a lifecycle tag.
/// </summary>
public class LifecycleTag
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

/// <summary>
/// Represents a lifecycle transition.
/// </summary>
public class LifecycleTransition
{
    /// <summary>
    /// Gets or sets the days after creation.
    /// </summary>
    public int Days { get; set; }

    /// <summary>
    /// Gets or sets the storage class.
    /// </summary>
    public string StorageClass { get; set; } = string.Empty;
}

/// <summary>
/// Represents a lifecycle expiration.
/// </summary>
public class LifecycleExpiration
{
    /// <summary>
    /// Gets or sets the days after creation.
    /// </summary>
    public int? Days { get; set; }

    /// <summary>
    /// Gets or sets the date.
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// Gets or sets whether to expire delete markers.
    /// </summary>
    public bool ExpiredObjectDeleteMarker { get; set; }
}

/// <summary>
/// Represents a lifecycle noncurrent version expiration.
/// </summary>
public class LifecycleNoncurrentVersionExpiration
{
    /// <summary>
    /// Gets or sets the days after becoming noncurrent.
    /// </summary>
    public int Days { get; set; }
}

/// <summary>
/// Represents a lifecycle abort incomplete multipart upload.
/// </summary>
public class LifecycleAbortIncompleteMultipartUpload
{
    /// <summary>
    /// Gets or sets the days after initiation.
    /// </summary>
    public int DaysAfterInitiation { get; set; }
}