namespace Ierahkwa.TelecomService.Domain;

public class Bandwidth
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
    public int AllocatedMbps { get; set; }
    public int UsedMbps { get; set; }
    public DateTime MeasuredAt { get; set; }
    public double UtilizationPercent { get; set; }
}
