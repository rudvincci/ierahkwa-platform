namespace Mamey.Government.Identity.Domain.Entities;

/// <summary>
/// Represents the status of a two-factor authentication setup.
/// </summary>
public enum TwoFactorAuthStatus
{
    /// <summary>
    /// 2FA is pending activation.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 2FA is active and can be used.
    /// </summary>
    Active = 2,

    /// <summary>
    /// 2FA is disabled.
    /// </summary>
    Disabled = 3
}