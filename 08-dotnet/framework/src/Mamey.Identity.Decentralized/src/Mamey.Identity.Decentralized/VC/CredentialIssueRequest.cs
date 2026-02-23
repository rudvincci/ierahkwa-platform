using System.ComponentModel.DataAnnotations;

namespace Mamey.Identity.Decentralized.VC;

/// <summary>
/// Request to issue a new Verifiable Credential (VC).
/// </summary>
public class CredentialIssueRequest
{
    [Required]
    public string IssuerDid { get; set; }

    [Required]
    public string SubjectDid { get; set; }

    [Required]
    public string CredentialType { get; set; } // e.g., "AlumniCredential"

    /// <summary>
    /// Claims or assertions made about the subject (attribute name -> value).
    /// </summary>
    [Required]
    public Dictionary<string, object> Claims { get; set; } = new();

    /// <summary>
    /// Credential status list URL or identifier.
    /// </summary>
    public string CredentialStatusId { get; set; }

    /// <summary>
    /// Expiry date of the credential (optional).
    /// </summary>
    public DateTimeOffset? Expiration { get; set; }

    /// <summary>
    /// Optional additional context URIs for custom vocabularies.
    /// </summary>
    public List<string> Contexts { get; set; } = new();

    /// <summary>
    /// Optional: custom proof options or signatures.
    /// </summary>
    public Dictionary<string, object> ProofOptions { get; set; } = new();
}