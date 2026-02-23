using NotifyHub.Core.Interfaces;
using NotifyHub.Core.Models;
namespace NotifyHub.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly List<Notification> _notifications = new();
    private readonly List<NotificationDelivery> _deliveries = new();
    private readonly List<NotificationTemplate> _templates = new();
    private readonly List<UserPreference> _preferences = new();
    private readonly List<Subscription> _subscriptions = new();

    public Task<Notification> SendNotificationAsync(Notification notification)
    {
        notification.Id = Guid.NewGuid(); notification.NotificationId = $"NTF-{DateTime.UtcNow:yyyyMMddHHmmss}-{_notifications.Count + 1:D5}"; notification.Status = NotificationStatus.Sending; notification.CreatedAt = DateTime.UtcNow; _notifications.Add(notification);
        if (notification.RecipientUserIds != null) foreach (var userId in notification.RecipientUserIds) foreach (var channel in notification.Channels) { _deliveries.Add(new NotificationDelivery { Id = Guid.NewGuid(), NotificationId = notification.Id, UserId = userId, Channel = channel, Status = DeliveryStatus.Delivered, DeliveredAt = DateTime.UtcNow }); }
        notification.Status = NotificationStatus.Sent; notification.SentAt = DateTime.UtcNow; return Task.FromResult(notification);
    }
    public Task<Notification> ScheduleNotificationAsync(Notification notification) { notification.Id = Guid.NewGuid(); notification.NotificationId = $"NTF-{_notifications.Count + 1:D5}"; notification.Status = NotificationStatus.Scheduled; notification.CreatedAt = DateTime.UtcNow; _notifications.Add(notification); return Task.FromResult(notification); }
    public Task<Notification?> GetNotificationByIdAsync(Guid id) => Task.FromResult(_notifications.FirstOrDefault(n => n.Id == id));
    public Task<IEnumerable<Notification>> GetNotificationsAsync(NotificationStatus? status = null, NotificationType? type = null) { var q = _notifications.AsEnumerable(); if (status.HasValue) q = q.Where(n => n.Status == status.Value); if (type.HasValue) q = q.Where(n => n.Type == type.Value); return Task.FromResult(q.OrderByDescending(n => n.CreatedAt)); }
    public Task<Notification> CancelNotificationAsync(Guid id) { var n = _notifications.FirstOrDefault(n => n.Id == id); if (n != null && n.Status == NotificationStatus.Scheduled) n.Status = NotificationStatus.Cancelled; return Task.FromResult(n!); }

    public Task<IEnumerable<NotificationDelivery>> GetUserNotificationsAsync(Guid userId, bool? unreadOnly = null, int page = 1, int pageSize = 20)
    {
        var q = _deliveries.Where(d => d.UserId == userId); if (unreadOnly == true) q = q.Where(d => !d.IsRead);
        return Task.FromResult(q.OrderByDescending(d => d.DeliveredAt).Skip((page - 1) * pageSize).Take(pageSize));
    }
    public Task<int> GetUnreadCountAsync(Guid userId) => Task.FromResult(_deliveries.Count(d => d.UserId == userId && !d.IsRead));
    public Task MarkAsReadAsync(Guid userId, Guid notificationId) { var d = _deliveries.FirstOrDefault(d => d.UserId == userId && d.NotificationId == notificationId); if (d != null) { d.IsRead = true; d.ReadAt = DateTime.UtcNow; } return Task.CompletedTask; }
    public Task MarkAllAsReadAsync(Guid userId) { foreach (var d in _deliveries.Where(d => d.UserId == userId && !d.IsRead)) { d.IsRead = true; d.ReadAt = DateTime.UtcNow; } return Task.CompletedTask; }
    public Task DeleteNotificationAsync(Guid userId, Guid notificationId) { _deliveries.RemoveAll(d => d.UserId == userId && d.NotificationId == notificationId); return Task.CompletedTask; }

    public Task<NotificationTemplate> CreateTemplateAsync(NotificationTemplate template) { template.Id = Guid.NewGuid(); template.CreatedAt = DateTime.UtcNow; _templates.Add(template); return Task.FromResult(template); }
    public Task<NotificationTemplate?> GetTemplateByIdAsync(Guid id) => Task.FromResult(_templates.FirstOrDefault(t => t.Id == id));
    public Task<NotificationTemplate?> GetTemplateByNameAsync(string name) => Task.FromResult(_templates.FirstOrDefault(t => t.TemplateName == name));
    public Task<IEnumerable<NotificationTemplate>> GetTemplatesAsync(NotificationType? type = null) => Task.FromResult(type.HasValue ? _templates.Where(t => t.Type == type.Value && t.IsActive) : _templates.Where(t => t.IsActive));
    public Task<NotificationTemplate> UpdateTemplateAsync(NotificationTemplate template) { var e = _templates.FirstOrDefault(t => t.Id == template.Id); if (e != null) { e.SubjectTemplate = template.SubjectTemplate; e.BodyTemplate = template.BodyTemplate; e.UpdatedAt = DateTime.UtcNow; } return Task.FromResult(e ?? template); }
    public async Task<Notification> SendFromTemplateAsync(string templateName, Dictionary<string, string> variables, List<Guid> recipientUserIds)
    {
        var template = await GetTemplateByNameAsync(templateName); if (template == null) throw new Exception("Template not found");
        var title = template.SubjectTemplate; var body = template.BodyTemplate; foreach (var v in variables) { title = title.Replace($"{{{{{v.Key}}}}}", v.Value); body = body.Replace($"{{{{{v.Key}}}}}", v.Value); }
        return await SendNotificationAsync(new Notification { Type = template.Type, Title = title, Body = body, Channels = template.DefaultChannels, RecipientUserIds = recipientUserIds });
    }

    public Task<UserPreference> GetUserPreferencesAsync(Guid userId) { var p = _preferences.FirstOrDefault(p => p.UserId == userId); if (p == null) { p = new UserPreference { Id = Guid.NewGuid(), UserId = userId, UpdatedAt = DateTime.UtcNow }; _preferences.Add(p); } return Task.FromResult(p); }
    public Task<UserPreference> UpdateUserPreferencesAsync(UserPreference preferences) { var e = _preferences.FirstOrDefault(p => p.UserId == preferences.UserId); if (e != null) { e.EmailEnabled = preferences.EmailEnabled; e.SmsEnabled = preferences.SmsEnabled; e.PushEnabled = preferences.PushEnabled; e.UpdatedAt = DateTime.UtcNow; } return Task.FromResult(e ?? preferences); }
    public Task RegisterPushTokenAsync(Guid userId, string token) { var p = _preferences.FirstOrDefault(p => p.UserId == userId); if (p != null) p.PushToken = token; return Task.CompletedTask; }

    public Task<Subscription> SubscribeToTopicAsync(Guid userId, string topic, List<DeliveryChannel> channels) { var s = new Subscription { Id = Guid.NewGuid(), UserId = userId, Topic = topic, Channels = channels, SubscribedAt = DateTime.UtcNow }; _subscriptions.Add(s); return Task.FromResult(s); }
    public Task UnsubscribeFromTopicAsync(Guid userId, string topic) { _subscriptions.RemoveAll(s => s.UserId == userId && s.Topic == topic); return Task.CompletedTask; }
    public Task<IEnumerable<Subscription>> GetUserSubscriptionsAsync(Guid userId) => Task.FromResult(_subscriptions.Where(s => s.UserId == userId && s.IsActive));
    public async Task BroadcastToTopicAsync(string topic, Notification notification) { var subs = _subscriptions.Where(s => s.Topic == topic && s.IsActive).ToList(); notification.RecipientUserIds = subs.Select(s => s.UserId).ToList(); await SendNotificationAsync(notification); }

    public Task ProcessScheduledNotificationsAsync() => Task.CompletedTask;
    public Task RetryFailedDeliveriesAsync() => Task.CompletedTask;

    public Task<NotificationStatistics> GetStatisticsAsync() => Task.FromResult(new NotificationStatistics { TotalSent = _notifications.Count(n => n.Status == NotificationStatus.Sent), SentToday = _notifications.Count(n => n.SentAt?.Date == DateTime.UtcNow.Date), Delivered = _deliveries.Count(d => d.Status == DeliveryStatus.Delivered), Failed = _deliveries.Count(d => d.Status == DeliveryStatus.Failed), Pending = _deliveries.Count(d => d.Status == DeliveryStatus.Pending), DeliveryRate = _deliveries.Any() ? _deliveries.Count(d => d.Status == DeliveryStatus.Delivered) * 100.0 / _deliveries.Count : 0, OpenRate = _deliveries.Any() ? _deliveries.Count(d => d.IsRead) * 100.0 / _deliveries.Count : 0, ByChannel = _deliveries.GroupBy(d => d.Channel.ToString()).ToDictionary(g => g.Key, g => g.Count()), ByType = _notifications.GroupBy(n => n.Type.ToString()).ToDictionary(g => g.Key, g => g.Count()) });
}
