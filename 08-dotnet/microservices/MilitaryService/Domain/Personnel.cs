namespace Ierahkwa.MilitaryService.Domain;

public class Personnel
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
    public string ServiceNumber { get; set; } = string.Empty;
    public string Rank { get; set; } = string.Empty;
    public Guid UnitId { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public DateTime EnlistmentDate { get; set; }
}
