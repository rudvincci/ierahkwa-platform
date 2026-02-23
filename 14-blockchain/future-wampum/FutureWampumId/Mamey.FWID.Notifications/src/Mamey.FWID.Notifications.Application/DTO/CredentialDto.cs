namespace Mamey.FWID.Notifications.Application.DTO;

/// <summary>
/// DTO for credential information from Credentials service.
/// </summary>
internal class CredentialDto
{
    public Guid CredentialId { get; set; }
    public Guid IdentityId { get; set; }
    public string CredentialType { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
}







