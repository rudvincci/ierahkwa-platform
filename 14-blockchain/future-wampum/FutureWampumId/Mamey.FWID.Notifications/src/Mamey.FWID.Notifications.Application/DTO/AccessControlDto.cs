namespace Mamey.FWID.Notifications.Application.DTO;

/// <summary>
/// DTO for access control information from AccessControls service.
/// </summary>
internal class AccessControlDto
{
    public Guid AccessControlId { get; set; }
    public Guid IdentityId { get; set; }
    public Guid ZoneId { get; set; }
    public string Permission { get; set; } = string.Empty;
    public DateTime GrantedAt { get; set; }
}







