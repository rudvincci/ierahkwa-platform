namespace Mamey.Government.Identity.Domain.Entities;

/// <summary>
/// Represents the status of a subject in the identity system.
/// </summary>
public enum SubjectStatus
{
    /// <summary>
    /// Subject is active and can authenticate.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Subject is inactive and cannot authenticate.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Subject is suspended temporarily.
    /// </summary>
    Suspended = 3,

    /// <summary>
    /// Subject is pending verification.
    /// </summary>
    Pending = 4,

    /// <summary>
    /// Subject is locked due to security reasons.
    /// </summary>
    Locked = 5
}