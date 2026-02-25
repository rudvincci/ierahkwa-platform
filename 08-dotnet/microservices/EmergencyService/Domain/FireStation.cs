namespace Ierahkwa.EmergencyService.Domain;

public class FireStation
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
    public string StationCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int TruckCount { get; set; }
    public int FirefighterCount { get; set; }
    public string District { get; set; } = string.Empty;
}
