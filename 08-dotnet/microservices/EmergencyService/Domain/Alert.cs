namespace Ierahkwa.EmergencyService.Domain;

public class Alert
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
    public string AlertLevel { get; set; } = "Warning";
    public string AlertType { get; set; } = string.Empty;
    public string AffectedArea { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsBroadcast { get; set; }
}
