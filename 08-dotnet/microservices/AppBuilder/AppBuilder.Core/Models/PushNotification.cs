namespace AppBuilder.Core.Models;

/// <summary>Push notification - Appy: Send to app users, schedule, images, delivery status.</summary>
public class PushNotification
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string AppProjectId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime? ScheduledAt { get; set; }      // null = send immediately
    public DateTime? SentAt { get; set; }
    public PushDeliveryStatus Status { get; set; } = PushDeliveryStatus.Pending;
    public int? TargetDeviceCount { get; set; }
    public int? DeliveredCount { get; set; }
    public string? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum PushDeliveryStatus
{
    Pending,
    Scheduled,
    Sending,
    Sent,
    Failed
}
