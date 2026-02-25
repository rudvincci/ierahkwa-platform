namespace Ierahkwa.SocialWelfareService.Domain;

public class UnemploymentClaim
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
    public Guid ClaimantId { get; set; }
    public string PreviousEmployer { get; set; } = string.Empty;
    public DateTime TerminationDate { get; set; }
    public decimal WeeklyBenefit { get; set; }
    public int RemainingWeeks { get; set; }
}
