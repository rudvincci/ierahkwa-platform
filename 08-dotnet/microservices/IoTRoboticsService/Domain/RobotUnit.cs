namespace Ierahkwa.IoTRoboticsService.Domain;

public class RobotUnit
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
    public string SerialNumber { get; set; } = string.Empty;
    public string RobotType { get; set; } = string.Empty;
    public double BatteryLevel { get; set; }
    public string CurrentTask { get; set; } = string.Empty;
}
