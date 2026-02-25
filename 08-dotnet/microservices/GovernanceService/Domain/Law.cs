namespace Ierahkwa.GovernanceService.Domain;

public class Law
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
    public string LawNumber { get; set; } = string.Empty;
    public DateTime EnactedDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public string FullText { get; set; } = string.Empty;
    public bool IsRepealed { get; set; }
}
