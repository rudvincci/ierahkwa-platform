namespace Ierahkwa.TransportService.Domain;

public class ShipRecord
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
    public string VesselName { get; set; } = string.Empty;
    public string IMONumber { get; set; } = string.Empty;
    public decimal Tonnage { get; set; }
    public string PortOfRegistry { get; set; } = string.Empty;
}
