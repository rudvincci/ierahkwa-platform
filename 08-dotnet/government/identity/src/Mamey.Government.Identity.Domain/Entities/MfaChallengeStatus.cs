namespace Mamey.Government.Identity.Domain.Entities;

/// <summary>
/// Represents the status of an MFA challenge.
/// </summary>
public enum MfaChallengeStatus
{
    /// <summary>
    /// Challenge is pending verification.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Challenge has been verified.
    /// </summary>
    Verified = 2,

    /// <summary>
    /// Challenge has expired.
    /// </summary>
    Expired = 3
}