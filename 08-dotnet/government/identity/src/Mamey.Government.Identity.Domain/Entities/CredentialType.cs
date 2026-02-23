namespace Mamey.Government.Identity.Domain.Entities;

/// <summary>
/// Represents the type of credential in the identity system.
/// </summary>
public enum CredentialType
{
    /// <summary>
    /// Password-based authentication.
    /// </summary>
    Password = 1,

    /// <summary>
    /// Biometric authentication (fingerprint, face, etc.).
    /// </summary>
    Biometric = 2,

    /// <summary>
    /// Hardware token authentication.
    /// </summary>
    HardwareToken = 3,

    /// <summary>
    /// Software token authentication (TOTP, etc.).
    /// </summary>
    SoftwareToken = 4,

    /// <summary>
    /// Certificate-based authentication.
    /// </summary>
    Certificate = 5,

    /// <summary>
    /// API key authentication.
    /// </summary>
    ApiKey = 6
}