namespace Mamey.FWID.Identities.Application.DTO;

/// <summary>
/// DTO for ZKP proof information from ZKPs service.
/// </summary>
internal class ZKPProofDto
{
    public Guid ProofId { get; set; }
    public Guid IdentityId { get; set; }
    public string AttributeType { get; set; } = string.Empty;
    public Dictionary<string, object> Attributes { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}


