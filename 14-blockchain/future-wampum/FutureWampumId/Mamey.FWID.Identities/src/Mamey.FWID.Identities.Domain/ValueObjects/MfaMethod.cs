namespace Mamey.FWID.Identities.Domain.ValueObjects;

/// <summary>
/// Represents the multi-factor authentication method.
/// </summary>
public enum MfaMethod
{
    /// <summary>
    /// Time-based One-Time Password (TOTP).
    /// </summary>
    Totp = 0,
    
    /// <summary>
    /// SMS-based verification code.
    /// </summary>
    Sms = 1,
    
    /// <summary>
    /// Email-based verification code.
    /// </summary>
    Email = 2,
    
    /// <summary>
    /// Biometric verification.
    /// </summary>
    Biometric = 3,
    
    /// <summary>
    /// Backup code for account recovery.
    /// </summary>
    BackupCode = 4
}

