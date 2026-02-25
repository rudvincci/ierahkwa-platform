namespace Ierahkwa.JusticeService.Domain;

public class Hearing
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
    public Guid CaseId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string Courtroom { get; set; } = string.Empty;
    public Guid JudgeId { get; set; }
    public bool IsVirtual { get; set; }
}
