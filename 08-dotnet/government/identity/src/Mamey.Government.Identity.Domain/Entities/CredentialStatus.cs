namespace Mamey.Government.Identity.Domain.Entities;

/// <summary>
/// Represents the status of a credential in the identity system.
/// </summary>
public enum CredentialStatus
{
    /// <summary>
    /// Credential is active and can be used for authentication.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Credential is inactive and cannot be used.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Credential has been revoked and cannot be used.
    /// </summary>
    Revoked = 3,

    /// <summary>
    /// Credential has expired and cannot be used.
    /// </summary>
    Expired = 4
}