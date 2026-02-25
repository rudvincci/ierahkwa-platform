namespace Ierahkwa.DroneService.Domain;

public class SensorData
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
    public Guid MissionId { get; set; }
    public string SensorType { get; set; } = string.Empty;
    public string DataPayload { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
