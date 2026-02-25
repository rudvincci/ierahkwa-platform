namespace Ierahkwa.NetworkService.Domain;

public class BandwidthAllocation
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
    public Guid NodeId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public int AllocatedMbps { get; set; }
    public string Priority { get; set; } = "Normal";
}
