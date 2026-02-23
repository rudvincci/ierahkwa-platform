namespace Mamey.FWID.Notifications.Application.DTO;

/// <summary>
/// DTO for ZKP proof information from ZKPs service.
/// </summary>
internal class ZKPProofDto
{
    public Guid ProofId { get; set; }
    public Guid IdentityId { get; set; }
    public string AttributeType { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}







