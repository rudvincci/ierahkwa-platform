using NotifyHub.Core.Models;
namespace NotifyHub.Core.Interfaces;

public interface INotificationService
{
    Task<Notification> SendNotificationAsync(Notification notification);
    Task<Notification> ScheduleNotificationAsync(Notification notification);
    Task<Notification?> GetNotificationByIdAsync(Guid id);
    Task<IEnumerable<Notification>> GetNotificationsAsync(NotificationStatus? status = null, NotificationType? type = null);
    Task<Notification> CancelNotificationAsync(Guid id);

    Task<IEnumerable<NotificationDelivery>> GetUserNotificationsAsync(Guid userId, bool? unreadOnly = null, int page = 1, int pageSize = 20);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task MarkAsReadAsync(Guid userId, Guid notificationId);
    Task MarkAllAsReadAsync(Guid userId);
    Task DeleteNotificationAsync(Guid userId, Guid notificationId);

    Task<NotificationTemplate> CreateTemplateAsync(NotificationTemplate template);
    Task<NotificationTemplate?> GetTemplateByIdAsync(Guid id);
    Task<NotificationTemplate?> GetTemplateByNameAsync(string name);
    Task<IEnumerable<NotificationTemplate>> GetTemplatesAsync(NotificationType? type = null);
    Task<NotificationTemplate> UpdateTemplateAsync(NotificationTemplate template);
    Task<Notification> SendFromTemplateAsync(string templateName, Dictionary<string, string> variables, List<Guid> recipientUserIds);

    Task<UserPreference> GetUserPreferencesAsync(Guid userId);
    Task<UserPreference> UpdateUserPreferencesAsync(UserPreference preferences);
    Task RegisterPushTokenAsync(Guid userId, string token);

    Task<Subscription> SubscribeToTopicAsync(Guid userId, string topic, List<DeliveryChannel> channels);
    Task UnsubscribeFromTopicAsync(Guid userId, string topic);
    Task<IEnumerable<Subscription>> GetUserSubscriptionsAsync(Guid userId);
    Task BroadcastToTopicAsync(string topic, Notification notification);

    Task ProcessScheduledNotificationsAsync();
    Task RetryFailedDeliveriesAsync();
    Task<NotificationStatistics> GetStatisticsAsync();
}

public class NotificationStatistics
{
    public int TotalSent { get; set; }
    public int SentToday { get; set; }
    public int Delivered { get; set; }
    public int Failed { get; set; }
    public int Pending { get; set; }
    public double DeliveryRate { get; set; }
    public double OpenRate { get; set; }
    public Dictionary<string, int> ByChannel { get; set; } = new();
    public Dictionary<string, int> ByType { get; set; } = new();
    public List<DailyStats> DailyTrend { get; set; } = new();
}

public class DailyStats { public DateTime Date { get; set; } public int Sent { get; set; } public int Delivered { get; set; } }
