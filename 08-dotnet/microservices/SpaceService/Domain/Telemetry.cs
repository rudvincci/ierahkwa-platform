namespace Ierahkwa.SpaceService.Domain;

public class Telemetry
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
    public Guid SatelliteId { get; set; }
    public double BatteryLevelPercent { get; set; }
    public double TemperatureCelsius { get; set; }
    public double SignalStrengthDbm { get; set; }
    public DateTime ReceivedAt { get; set; }
}
