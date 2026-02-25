namespace Ierahkwa.SpaceService.Domain;

public class OrbitData
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
    public double Inclination { get; set; }
    public double Eccentricity { get; set; }
    public double PeriodMinutes { get; set; }
    public string TleData { get; set; } = string.Empty;
}
