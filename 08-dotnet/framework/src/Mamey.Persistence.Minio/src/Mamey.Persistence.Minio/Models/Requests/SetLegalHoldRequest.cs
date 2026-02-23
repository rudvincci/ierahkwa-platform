using Mamey.Persistence.Minio.Models.DTOs;

namespace Mamey.Persistence.Minio.Models.Requests;

/// <summary>
/// Request for setting legal hold.
/// </summary>
public class SetLegalHoldRequest
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
    /// Gets or sets the legal hold status.
    /// </summary>
    public LegalHoldStatus Status { get; set; }
}
