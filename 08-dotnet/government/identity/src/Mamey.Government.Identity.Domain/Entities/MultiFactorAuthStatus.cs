namespace Mamey.Government.Identity.Domain.Entities;

/// <summary>
/// Represents the status of a multi-factor authentication setup.
/// </summary>
public enum MultiFactorAuthStatus
{
    /// <summary>
    /// MFA is inactive (insufficient methods enabled).
    /// </summary>
    Inactive = 1,

    /// <summary>
    /// MFA is active and can be used.
    /// </summary>
    Active = 2,

    /// <summary>
    /// MFA is disabled.
    /// </summary>
    Disabled = 3
}