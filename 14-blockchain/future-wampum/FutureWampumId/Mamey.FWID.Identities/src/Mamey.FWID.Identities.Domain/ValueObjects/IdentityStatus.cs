namespace Mamey.FWID.Identities.Domain.ValueObjects;

/// <summary>
/// Represents the status of an identity.
/// </summary>
public enum IdentityStatus
{
    /// <summary>
    /// Identity is pending verification.
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Identity has been verified.
    /// </summary>
    Verified = 1,
    
    /// <summary>
    /// Identity has been revoked.
    /// </summary>
    Revoked = 2,
    
    /// <summary>
    /// Identity has been suspended.
    /// </summary>
    Suspended = 3
}

