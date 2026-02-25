namespace Ierahkwa.DroneService.Domain;

public class Mission
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
    public Guid FlightPlanId { get; set; }
    public string Objective { get; set; } = string.Empty;
    public string Priority { get; set; } = "Normal";
    public Guid OperatorId { get; set; }
}
