namespace Ierahkwa.NetworkService.Domain;

public class CDNEdge
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
    public string Location { get; set; } = string.Empty;
    public long CacheSizeGB { get; set; }
    public double HitRate { get; set; }
    public string Endpoint { get; set; } = string.Empty;
}
