using Mamey.Persistence.Minio.Models.DTOs;

namespace Mamey.Persistence.Minio.Models.Requests;

/// <summary>
/// Request for setting object retention.
/// </summary>
public class SetObjectRetentionRequest
{
    /// <summary>
    /// Gets or sets the bucket name.
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the object name.
    /// </summary>
    public string ObjectName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version ID.
    /// </summary>
    public string? VersionId { get; set; }

    /// <summary>
    /// Gets or sets the retention mode.
    /// </summary>
    public ObjectRetentionMode Mode { get; set; }

    /// <summary>
    /// Gets or sets the retain until date.
    /// </summary>
    public DateTime RetainUntilDate { get; set; }

    /// <summary>
    /// Gets or sets whether to bypass governance mode.
    /// </summary>
    public bool BypassGovernanceMode { get; set; }
}
