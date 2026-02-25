namespace Ierahkwa.IntelligenceService.Domain;

public class ThreatAssessment
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
    public string ThreatLevel { get; set; } = "Moderate";
    public string Region { get; set; } = string.Empty;
    public string Analysis { get; set; } = string.Empty;
    public DateTime AssessmentDate { get; set; }
    public DateTime NextReviewDate { get; set; }
}
