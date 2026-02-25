namespace Ierahkwa.SocialWelfareService.Domain;

public class DisabilityRecord
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
    public Guid PersonId { get; set; }
    public string DisabilityType { get; set; } = string.Empty;
    public int DisabilityPercentage { get; set; }
    public string AssistanceRequired { get; set; } = string.Empty;
    public DateTime AssessmentDate { get; set; }
}
