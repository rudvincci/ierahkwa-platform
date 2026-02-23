using System.ComponentModel.DataAnnotations;

namespace Mamey.Identity.Decentralized.VC;

/// <summary>
/// Request to update the revocation/status of a Verifiable Credential.
/// </summary>
public class CredentialStatusUpdateRequest
{
    [Required]
    public string CredentialId { get; set; }
    [Required]
    public string Status { get; set; } // "revoked", "active", etc.
    public string Reason { get; set; }
}