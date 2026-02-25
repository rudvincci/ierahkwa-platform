namespace Ierahkwa.SearchService.Domain;

public class SearchQuery
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
    public string QueryText { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public int ResultCount { get; set; }
    public int LatencyMs { get; set; }
}
