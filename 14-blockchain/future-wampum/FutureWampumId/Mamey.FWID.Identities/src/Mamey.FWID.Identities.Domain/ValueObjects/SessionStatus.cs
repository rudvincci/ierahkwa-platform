namespace Mamey.FWID.Identities.Domain.ValueObjects;

/// <summary>
/// Represents the status of a session.
/// </summary>
public enum SessionStatus
{
    /// <summary>
    /// Session is active and valid.
    /// </summary>
    Active = 0,
    
    /// <summary>
    /// Session has expired.
    /// </summary>
    Expired = 1,
    
    /// <summary>
    /// Session has been revoked.
    /// </summary>
    Revoked = 2
}

