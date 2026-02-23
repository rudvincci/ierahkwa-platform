namespace NotifyHub.Core.Models;

public class Notification
{
    public Guid Id { get; set; }
    public string NotificationId { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? RichContent { get; set; }
    public string? Icon { get; set; }
    public string? ActionUrl { get; set; }
    public string? Data { get; set; }
    public string? Category { get; set; }
    public Guid? SenderId { get; set; }
    public string? SenderName { get; set; }
    public string? SenderSystem { get; set; }
    public List<Guid>? RecipientUserIds { get; set; }
    public List<string>? RecipientGroups { get; set; }
    public string? RecipientDepartment { get; set; }
    public bool IsBroadcast { get; set; }
    public List<DeliveryChannel> Channels { get; set; } = new();
    public DateTime? ScheduledAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
}

public class NotificationDelivery
{
    public Guid Id { get; set; }
    public Guid NotificationId { get; set; }
    public Guid UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? UserPhone { get; set; }
    public DeliveryChannel Channel { get; set; }
    public DeliveryStatus Status { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
}

public class NotificationTemplate
{
    public Guid Id { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public NotificationType Type { get; set; }
    public string SubjectTemplate { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public string? HtmlTemplate { get; set; }
    public List<DeliveryChannel> DefaultChannels { get; set; } = new();
    public string? Variables { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UserPreference
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? PushToken { get; set; }
    public bool EmailEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; }
    public bool PushEnabled { get; set; } = true;
    public bool InAppEnabled { get; set; } = true;
    public string? QuietHoursStart { get; set; }
    public string? QuietHoursEnd { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? MutedCategories { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class Subscription
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Topic { get; set; } = string.Empty;
    public List<DeliveryChannel> Channels { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime SubscribedAt { get; set; }
}

public enum NotificationType { System, Alert, Reminder, Update, Message, Task, Approval, Warning, Error, Info, Marketing }
public enum NotificationPriority { Low, Normal, High, Urgent }
public enum NotificationStatus { Draft, Scheduled, Sending, Sent, Failed, Cancelled }
public enum DeliveryChannel { InApp, Email, SMS, Push, Webhook, Teams, Slack }
public enum DeliveryStatus { Pending, Queued, Sent, Delivered, Failed, Bounced }
