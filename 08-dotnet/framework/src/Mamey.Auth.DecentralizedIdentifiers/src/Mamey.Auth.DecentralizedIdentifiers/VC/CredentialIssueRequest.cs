using System.ComponentModel.DataAnnotations;

namespace Mamey.Auth.DecentralizedIdentifiers.VC;

/// <summary>
/// Enhanced request to issue a new Verifiable Credential (VC).
/// </summary>
public class CredentialIssueRequest
{
    /// <summary>
    /// The DID of the credential issuer.
    /// </summary>
    [Required]
    public string IssuerDid { get; set; } = string.Empty;

    /// <summary>
    /// The DID of the credential subject (holder).
    /// </summary>
    [Required]
    public string SubjectDid { get; set; } = string.Empty;

    /// <summary>
    /// The type of credential (e.g., "AlumniCredential", "IdentityCredential").
    /// </summary>
    [Required]
    public string CredentialType { get; set; } = string.Empty;

    /// <summary>
    /// Claims or assertions made about the subject (attribute name -> value).
    /// </summary>
    [Required]
    public Dictionary<string, object> Claims { get; set; } = new();

    /// <summary>
    /// Credential status list URL or identifier.
    /// </summary>
    public string CredentialStatusId { get; set; } = string.Empty;

    /// <summary>
    /// Expiry date of the credential (optional).
    /// </summary>
    public DateTimeOffset? Expiration { get; set; }

    /// <summary>
    /// Issuance date of the credential (defaults to now).
    /// </summary>
    public DateTimeOffset? IssuanceDate { get; set; }

    /// <summary>
    /// Optional additional context URIs for custom vocabularies.
    /// </summary>
    public List<string> Contexts { get; set; } = new();

    /// <summary>
    /// Optional: custom proof options or signatures.
    /// </summary>
    public Dictionary<string, object> ProofOptions { get; set; } = new();

    /// <summary>
    /// Proof type to use for signing the credential.
    /// </summary>
    public ProofType ProofType { get; set; } = ProofType.Ed25519Signature2020;

    /// <summary>
    /// Credential schema reference.
    /// </summary>
    public string SchemaRef { get; set; } = string.Empty;

    /// <summary>
    /// Evidence references for the credential.
    /// </summary>
    public List<string> EvidenceRefs { get; set; } = new();

    /// <summary>
    /// Terms of use for the credential.
    /// </summary>
    public List<string> TermsOfUse { get; set; } = new();

    /// <summary>
    /// Refresh service information.
    /// </summary>
    public RefreshService RefreshService { get; set; }

    /// <summary>
    /// Custom credential ID (if not provided, will be generated).
    /// </summary>
    public string CredentialId { get; set; } = string.Empty;

    /// <summary>
    /// Whether to include credential status in the issued credential.
    /// </summary>
    public bool IncludeStatus { get; set; } = true;

    /// <summary>
    /// Whether to validate the credential schema before issuance.
    /// </summary>
    public bool ValidateSchema { get; set; } = true;

    /// <summary>
    /// Additional metadata for the credential.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Refresh service information for credential renewal.
/// </summary>
public class RefreshService
{
    /// <summary>
    /// The refresh service ID.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The refresh service type.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The refresh service endpoint URL.
    /// </summary>
    public string ServiceEndpoint { get; set; } = string.Empty;
}