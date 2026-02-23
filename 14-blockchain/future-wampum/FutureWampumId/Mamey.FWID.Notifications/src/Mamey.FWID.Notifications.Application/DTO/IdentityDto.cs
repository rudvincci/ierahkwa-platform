namespace Mamey.FWID.Notifications.Application.DTO;

/// <summary>
/// DTO for identity information from Identities service.
/// </summary>
internal class IdentityDto
{
    public Guid IdentityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Zone { get; set; }
    public DateTime CreatedAt { get; set; }
}







