namespace Ierahkwa.TelecomService.Domain;

public class ServicePlan
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
    public decimal MonthlyPrice { get; set; }
    public int DataLimitGb { get; set; }
    public int VoiceMinutes { get; set; }
    public int SmsLimit { get; set; }
    public string PlanType { get; set; } = "Prepaid";
}
