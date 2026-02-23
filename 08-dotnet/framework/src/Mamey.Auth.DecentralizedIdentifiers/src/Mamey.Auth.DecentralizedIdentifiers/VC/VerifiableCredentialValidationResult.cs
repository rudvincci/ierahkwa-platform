using System.Security.Claims;

namespace Mamey.Auth.DecentralizedIdentifiers.VC;

/// <summary>
/// Result of a credential or presentation verification.
/// </summary>
public class VerifiableCredentialValidationResult
{
    public bool IsValid { get; set; }
    public string CredentialId { get; set; }
    public string IssuerDid { get; set; }
    public string SubjectDid { get; set; }
    public string CredentialType { get; set; }
    public string PresentationId { get; set; }
    public List<Claim> Claims { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool IsRevoked { get; set; }
    public string RevocationReason { get; set; }

    /// <summary>
    /// Convenience: convert claims to dictionary.
    /// </summary>
    public IDictionary<string, string> GetClaims()
    {
        var dict = new Dictionary<string, string>();
        foreach (var claim in Claims)
            dict[claim.Type] = claim.Value;
        return dict;
    }
}