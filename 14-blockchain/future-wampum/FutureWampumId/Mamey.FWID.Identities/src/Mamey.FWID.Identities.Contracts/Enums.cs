namespace Mamey.FWID.Identities.Contracts;

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

/// <summary>
/// Represents the type of biometric authentication.
/// </summary>
public enum BiometricType
{
    Fingerprint = 0,
    FaceRecognition = 1,
    IrisRecognition = 2,
    VoiceRecognition = 3
}

/// <summary>
/// Represents the quality level of a biometric scan.
/// </summary>
public enum BiometricQuality
{
    Poor = 0,
    Fair = 1,
    Good = 2,
    Excellent = 3
}

/// <summary>
/// Represents the status of a session.
/// </summary>
public enum SessionStatus
{
    Active = 0,
    Expired = 1,
    Revoked = 2
}

/// <summary>
/// Represents the status of an email or SMS confirmation.
/// </summary>
public enum ConfirmationStatus
{
    Pending = 0,
    Confirmed = 1,
    Expired = 2
}

/// <summary>
/// Represents MFA authentication methods.
/// </summary>
public enum MfaMethod
{
    None = 0,
    Totp = 1,
    Sms = 2,
    Email = 3,
    Authenticator = 4
}

/// <summary>
/// Represents the status of a permission.
/// </summary>
public enum PermissionStatus
{
    Active = 0,
    Inactive = 1
}

/// <summary>
/// Represents the status of a role.
/// </summary>
public enum RoleStatus
{
    Active = 0,
    Inactive = 1
}








