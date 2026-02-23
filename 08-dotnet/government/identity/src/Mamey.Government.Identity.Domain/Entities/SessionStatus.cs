namespace Mamey.Government.Identity.Domain.Entities;

/// <summary>
/// Represents the status of a session in the identity system.
/// </summary>
public enum SessionStatus
{
    /// <summary>
    /// Session is active and can be used for authentication.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Session has been revoked and cannot be used.
    /// </summary>
    Revoked = 2,

    /// <summary>
    /// Session has expired and cannot be used.
    /// </summary>
    Expired = 3
}