namespace Mamey.Persistence.Minio.Models.DTOs;

/// <summary>
/// Represents object lock configuration for a bucket.
/// </summary>
public class ObjectLockConfiguration
{
    /// <summary>
    /// Gets or sets the object lock enabled status.
    /// </summary>
    public bool ObjectLockEnabled { get; set; }

    /// <summary>
    /// Gets or sets the default retention.
    /// </summary>
    public DefaultRetention? DefaultRetention { get; set; }
}

/// <summary>
/// Represents default retention settings.
/// </summary>
public class DefaultRetention
{
    /// <summary>
    /// Gets or sets the retention mode.
    /// </summary>
    public ObjectRetentionMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the number of days.
    /// </summary>
    public int? Days { get; set; }

    /// <summary>
    /// Gets or sets the number of years.
    /// </summary>
    public int? Years { get; set; }
}

/// <summary>
/// Represents object retention information.
/// </summary>
public class ObjectRetention
{
    /// <summary>
    /// Gets or sets the retention mode.
    /// </summary>
    public ObjectRetentionMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the retain until date.
    /// </summary>
    public DateTime RetainUntilDate { get; set; }
}

/// <summary>
/// Represents object retention modes.
/// </summary>
public enum ObjectRetentionMode
{
    /// <summary>
    /// Governance mode.
    /// </summary>
    Governance,

    /// <summary>
    /// Compliance mode.
    /// </summary>
    Compliance
}

/// <summary>
/// Represents legal hold status.
/// </summary>
public enum LegalHoldStatus
{
    /// <summary>
    /// Legal hold is on.
    /// </summary>
    On,

    /// <summary>
    /// Legal hold is off.
    /// </summary>
    Off
}
