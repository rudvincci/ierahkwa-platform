using System.ComponentModel.DataAnnotations;

namespace Mamey.Identity.Decentralized.VC;

/// <summary>
/// Request to verify a Verifiable Credential or Presentation.
/// </summary>
public class CredentialVerifyRequest
{
    [Required]
    public string CredentialJwt { get; set; } // Could also be LD-Proof JSON, but usually JWT

    /// <summary>
    /// Optional: expected subject DID for correlation.
    /// </summary>
    public string SubjectDid { get; set; }

    /// <summary>
    /// Optional: expected issuer DID for trust anchor validation.
    /// </summary>
    public string IssuerDid { get; set; }

    /// <summary>
    /// Optional: challenge or nonce for proof-of-possession.
    /// </summary>
    public string Challenge { get; set; }
}