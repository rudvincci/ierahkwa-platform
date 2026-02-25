namespace Ierahkwa.TelecomService.Domain;

public class CallRecord
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
    public string CallerNumber { get; set; } = string.Empty;
    public string ReceiverNumber { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public DateTime CallStartTime { get; set; }
    public string CallType { get; set; } = "Voice";
}
