using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Application.DTO;

/// <summary>
/// DTO for credential information from Credentials service.
/// </summary>
internal class CredentialDto
{
    public Guid CredentialId { get; set; }
    public IdentityId IdentityId { get; set; } = null!;
    public string CredentialType { get; set; } = null!;
    public Dictionary<string, object> Claims { get; set; } = new();
    public Guid IssuerId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }
    public Guid? RevokedBy { get; set; }
}
