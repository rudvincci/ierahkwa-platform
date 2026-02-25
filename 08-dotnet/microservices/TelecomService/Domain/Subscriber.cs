namespace Ierahkwa.TelecomService.Domain;

public class Subscriber
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
    public string PhoneNumber { get; set; } = string.Empty;
    public string SimCardId { get; set; } = string.Empty;
    public Guid ServicePlanId { get; set; }
    public DateTime ActivationDate { get; set; }
    public decimal Balance { get; set; }
}
