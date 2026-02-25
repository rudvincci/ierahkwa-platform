namespace Ierahkwa.NexusAggregationService.Domain;

public class DashboardWidget
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
    public string WidgetType { get; set; } = string.Empty;
    public string DataSource { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string Config { get; set; } = "{}";
}
