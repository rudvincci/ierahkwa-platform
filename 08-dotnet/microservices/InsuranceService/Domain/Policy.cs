namespace Ierahkwa.InsuranceService.Domain;

public class Policy
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
    public string PolicyNumber { get; set; } = string.Empty;
    public string PolicyType { get; set; } = string.Empty;
    public decimal Premium { get; set; }
    public decimal CoverageAmount { get; set; }
    public DateTime ExpiryDate { get; set; }
}
