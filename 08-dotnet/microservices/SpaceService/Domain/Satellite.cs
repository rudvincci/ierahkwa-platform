namespace Ierahkwa.SpaceService.Domain;

public class Satellite
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
    public string NoradId { get; set; } = string.Empty;
    public string OrbitType { get; set; } = string.Empty;
    public double AltitudeKm { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public DateTime LaunchDate { get; set; }
}
