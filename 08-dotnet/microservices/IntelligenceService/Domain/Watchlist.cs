namespace Ierahkwa.IntelligenceService.Domain;

public class Watchlist
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
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectType { get; set; } = string.Empty;
    public string RiskLevel { get; set; } = "High";
    public string Aliases { get; set; } = string.Empty;
    public string LastKnownLocation { get; set; } = string.Empty;
}
