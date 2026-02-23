namespace Mamey.FWID.Identities.Domain.ValueObjects;

/// <summary>
/// Represents the status of an email or SMS confirmation.
/// </summary>
public enum ConfirmationStatus
{
    /// <summary>
    /// Confirmation is pending.
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Confirmation has been confirmed.
    /// </summary>
    Confirmed = 1,
    
    /// <summary>
    /// Confirmation has expired.
    /// </summary>
    Expired = 2
}

