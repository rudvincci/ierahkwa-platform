namespace Ierahkwa.IoTRoboticsService.Domain;

public class DigitalTwin
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
    public Guid DeviceId { get; set; }
    public string ModelData { get; set; } = "{}";
    public DateTime LastSync { get; set; }
    public string SimulationState { get; set; } = string.Empty;
}
