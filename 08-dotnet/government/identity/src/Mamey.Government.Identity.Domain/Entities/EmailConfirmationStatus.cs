namespace Mamey.Government.Identity.Domain.Entities;

/// <summary>
/// Represents the status of an email confirmation.
/// </summary>
public enum EmailConfirmationStatus
{
    /// <summary>
    /// Confirmation is pending.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Email has been confirmed.
    /// </summary>
    Confirmed = 2,

    /// <summary>
    /// Confirmation has expired.
    /// </summary>
    Expired = 3
}