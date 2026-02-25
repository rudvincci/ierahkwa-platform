namespace Ierahkwa.DiplomacyService.Domain;

public class DiplomaticMission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public string CreatedBy { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string MissionType { get; set; } = string.Empty;
    public string DestinationCountry { get; set; } = string.Empty;
    public DateTime DepartureDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string LeadDiplomat { get; set; } = string.Empty;
}
