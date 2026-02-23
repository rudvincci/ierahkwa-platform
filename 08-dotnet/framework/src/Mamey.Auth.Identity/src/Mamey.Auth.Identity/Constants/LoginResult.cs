namespace Mamey.Auth.Identity.Constants;

/// Outcome of a sign‑in attempt.
/// </summary>
public enum LoginResult
{
    Success,
    Failed,
    MfaRequired,
    LockedOut
}