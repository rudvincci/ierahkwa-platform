namespace Ierahkwa.NetworkService.Domain;

public class Connection
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
    public Guid SourceNodeId { get; set; }
    public Guid TargetNodeId { get; set; }
    public int BandwidthMbps { get; set; }
    public int LatencyMs { get; set; }
}
