namespace Mamey.FWID.Identities.Application.DTO;

/// <summary>
/// DTO for DID information from DIDs service.
/// </summary>
internal class DIDDto
{
    public Guid DIDId { get; set; }
    public Guid IdentityId { get; set; }
    public string DidString { get; set; } = string.Empty;
    public string DIDMethod { get; set; } = string.Empty;
    public string? DocumentSignature { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}


