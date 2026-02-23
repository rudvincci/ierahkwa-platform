namespace Mamey.Government.Identity.Domain.Entities;

/// <summary>
/// Represents the status of a user in the identity system.
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// User is active and can authenticate.
    /// </summary>
    Active = 1,

    /// <summary>
    /// User is inactive and cannot authenticate.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// User is locked due to security reasons.
    /// </summary>
    Locked = 3,

    /// <summary>
    /// User is pending verification.
    /// </summary>
    Pending = 4
}