namespace Mamey.Government.Identity.Domain.Entities;

/// <summary>
/// Represents the types of MFA methods available.
/// </summary>
public enum MfaMethod
{
    /// <summary>
    /// Time-based One-Time Password (TOTP).
    /// </summary>
    Totp = 1,

    /// <summary>
    /// SMS-based verification.
    /// </summary>
    Sms = 2,

    /// <summary>
    /// Email-based verification.
    /// </summary>
    Email = 3,

    /// <summary>
    /// Push notification.
    /// </summary>
    Push = 4,

    /// <summary>
    /// Biometric authentication.
    /// </summary>
    Biometric = 5,

    /// <summary>
    /// Hardware security key.
    /// </summary>
    HardwareKey = 6,

    /// <summary>
    /// Voice call verification.
    /// </summary>
    Voice = 7
}