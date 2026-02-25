namespace Ierahkwa.InsuranceService.Domain;

public class BasicIncomeRecipient
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
    public string CitizenId { get; set; } = string.Empty;
    public decimal MonthlyAmount { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}
