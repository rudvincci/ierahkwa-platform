namespace Mamey.Auth.DecentralizedIdentifiers.VC;

/// <summary>
/// Result object for credential status checks.
/// </summary>
public class CredentialStatusResult
{
    /// <summary>
    /// True if the credential is revoked, false otherwise.
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// True if the credential is suspended (not always supported).
    /// </summary>
    public bool IsSuspended { get; set; }

    /// <summary>
    /// Optional textual reason for revocation/suspension.
    /// </summary>
    public string Reason { get; set; }

    /// <summary>
    /// Source or resolver that provided the status.
    /// </summary>
    public string Source { get; set; }
}