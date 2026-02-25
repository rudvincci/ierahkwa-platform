namespace Ierahkwa.InsuranceService.Domain;

public class Beneficiary
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
    public string RelationshipType { get; set; } = string.Empty;
    public Guid PolicyId { get; set; }
    public decimal SharePercentage { get; set; }
    public string ContactInfo { get; set; } = string.Empty;
}
