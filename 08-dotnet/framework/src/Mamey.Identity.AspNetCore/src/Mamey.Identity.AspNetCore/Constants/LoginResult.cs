namespace Mamey.Identity.AspNetCore.Constants;

/// <summary>
/// Outcome of a sign‑in attempt.
/// </summary>
public enum LoginResult
{
    Success,
    Failed,
    MfaRequired,
    LockedOut
}
