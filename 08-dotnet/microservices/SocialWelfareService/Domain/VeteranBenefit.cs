namespace Ierahkwa.SocialWelfareService.Domain;

public class VeteranBenefit
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
    public Guid VeteranId { get; set; }
    public string ServiceBranch { get; set; } = string.Empty;
    public int YearsOfService { get; set; }
    public string BenefitType { get; set; } = string.Empty;
    public decimal MonthlyAmount { get; set; }
}
