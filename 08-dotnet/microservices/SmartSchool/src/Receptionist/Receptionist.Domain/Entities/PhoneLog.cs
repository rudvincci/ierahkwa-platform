using Common.Domain.Entities;

namespace Receptionist.Domain.Entities;

public class PhoneLog : TenantEntity
{
    public string CallerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public CallType CallType { get; set; }
    public DateTime CallDate { get; set; } = DateTime.UtcNow;
    public TimeSpan? Duration { get; set; }
    public string? Purpose { get; set; }
    public string? Notes { get; set; }
    public string? FollowUp { get; set; }
    public int? ReceivedBy { get; set; }
}

public enum CallType
{
    Incoming,
    Outgoing,
    Missed
}
