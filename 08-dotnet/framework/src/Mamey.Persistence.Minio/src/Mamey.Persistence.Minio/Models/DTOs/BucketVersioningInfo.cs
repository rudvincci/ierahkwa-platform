namespace Mamey.Persistence.Minio.Models.DTOs;

/// <summary>
/// Represents the versioning configuration of a bucket.
/// </summary>
public class BucketVersioningInfo
{
    /// <summary>
    /// Gets or sets the versioning status.
    /// </summary>
    public VersioningStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the MFA delete status.
    /// </summary>
    public MfaDeleteStatus MfaDelete { get; set; }
}

/// <summary>
/// Represents the versioning status of a bucket.
/// </summary>
public enum VersioningStatus
{
    /// <summary>
    /// Versioning is not configured.
    /// </summary>
    Off,

    /// <summary>
    /// Versioning is enabled.
    /// </summary>
    Enabled,

    /// <summary>
    /// Versioning is suspended.
    /// </summary>
    Suspended
}

/// <summary>
/// Represents the MFA delete status.
/// </summary>
public enum MfaDeleteStatus
{
    /// <summary>
    /// MFA delete is disabled.
    /// </summary>
    Disabled,

    /// <summary>
    /// MFA delete is enabled.
    /// </summary>
    Enabled
}
