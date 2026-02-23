namespace Mamey.FWID.Notifications.Models;

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string NotificationId { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;
    
    // Content
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? RichContent { get; set; }
    public string? Icon { get; set; }
    public string? ActionUrl { get; set; }
    public Dictionary<string, string>? Data { get; set; }
    
    // Recipients
    public List<string> RecipientIds { get; set; } = new();
    public List<string>? RecipientEmails { get; set; }
    public List<string>? RecipientPhones { get; set; }
    public string? RecipientGroup { get; set; }
    public bool IsBroadcast { get; set; }
    
    // Channels
    public List<NotificationChannel> Channels { get; set; } = new() { NotificationChannel.InApp };
    
    // Sender
    public string? SenderId { get; set; }
    public string? SenderName { get; set; }
    public string? SenderSystem { get; set; }
    
    // Saga integration
    public string? SagaId { get; set; }
    public string? OperationId { get; set; }
    
    // Scheduling
    public DateTime? ScheduledAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    
    // Status
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
    public int RetryCount { get; set; }
}

public class NotificationDelivery
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid NotificationId { get; set; }
    public string RecipientId { get; set; } = string.Empty;
    public NotificationChannel Channel { get; set; }
    public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? ErrorMessage { get; set; }
}

public class NotificationTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TemplateName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public NotificationType Type { get; set; }
    public string SubjectTemplate { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public string? HtmlTemplate { get; set; }
    public List<NotificationChannel> DefaultChannels { get; set; } = new();
    public List<string>? Variables { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UserPreferences
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserId { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? PushToken { get; set; }
    public bool EmailEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; } = true;
    public bool PushEnabled { get; set; } = true;
    public bool InAppEnabled { get; set; } = true;
    public string? QuietHoursStart { get; set; }
    public string? QuietHoursEnd { get; set; }
    public List<string>? MutedCategories { get; set; }
}

public enum NotificationType { System, Alert, Reminder, Update, Message, Task, Approval, Warning, Error, Info, Marketing, Transaction, Governance, Treasury }
public enum NotificationPriority { Low, Normal, High, Urgent, Critical }
public enum NotificationStatus { Pending, Scheduled, Sending, Sent, Failed, Cancelled }
public enum NotificationChannel { InApp, Email, SMS, Push, Webhook, Telegram, WhatsApp }
public enum DeliveryStatus { Pending, Queued, Sent, Delivered, Failed, Bounced, Read }
