namespace Ierahkwa.JusticeService.Domain;

public class Sentence
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
    public string SentenceType { get; set; } = string.Empty;
    public int DurationMonths { get; set; }
    public DateTime SentenceDate { get; set; }
    public string IssuedByJudge { get; set; } = string.Empty;
}
