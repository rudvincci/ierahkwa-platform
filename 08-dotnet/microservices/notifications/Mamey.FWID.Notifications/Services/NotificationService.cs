using Mamey.FWID.Notifications.Models;

namespace Mamey.FWID.Notifications.Services;

public interface INotificationService
{
    Task<Notification> SendAsync(SendNotificationRequest request);
    Task<Notification> SendFromTemplateAsync(string templateName, Dictionary<string, string> variables, List<string> recipientIds);
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false);
    Task<bool> SendSmsAsync(string to, string message);
    Task<bool> SendPushAsync(string token, string title, string body, Dictionary<string, string>? data = null);
    Task<Notification?> GetAsync(Guid id);
    Task<IReadOnlyList<Notification>> GetForUserAsync(string userId, int limit = 50);
    Task<bool> MarkAsReadAsync(Guid id, string userId);
    Task<bool> MarkAllAsReadAsync(string userId);
    Task<NotificationTemplate> CreateTemplateAsync(NotificationTemplate template);
    Task<NotificationTemplate?> GetTemplateAsync(string templateName);
    Task<UserPreferences> GetPreferencesAsync(string userId);
    Task<UserPreferences> UpdatePreferencesAsync(string userId, UserPreferences preferences);
}

public class NotificationService : INotificationService
{
    private readonly Dictionary<Guid, Notification> _notifications = new();
    private readonly Dictionary<string, NotificationTemplate> _templates = new();
    private readonly Dictionary<string, UserPreferences> _preferences = new();
    private readonly Dictionary<Guid, List<NotificationDelivery>> _deliveries = new();
    private long _counter = 1000000;

    public NotificationService()
    {
        InitializeDefaultTemplates();
    }

    private void InitializeDefaultTemplates()
    {
        var templates = new[]
        {
            new NotificationTemplate
            {
                TemplateName = "WELCOME",
                Type = NotificationType.System,
                SubjectTemplate = "Welcome to Ierahkwa Sovereign Network, {{name}}!",
                BodyTemplate = "Your FutureWampumID is: {{fwid}}. Welcome to the sovereign ecosystem.",
                DefaultChannels = new() { NotificationChannel.Email, NotificationChannel.InApp }
            },
            new NotificationTemplate
            {
                TemplateName = "TRANSACTION",
                Type = NotificationType.Transaction,
                SubjectTemplate = "Transaction {{status}}: {{amount}} {{token}}",
                BodyTemplate = "Your transaction of {{amount}} {{token}} to {{recipient}} has been {{status}}.",
                DefaultChannels = new() { NotificationChannel.Push, NotificationChannel.InApp }
            },
            new NotificationTemplate
            {
                TemplateName = "GOVERNANCE_VOTE",
                Type = NotificationType.Governance,
                SubjectTemplate = "New Governance Proposal: {{title}}",
                BodyTemplate = "A new proposal requires your vote: {{title}}. Voting ends {{endDate}}.",
                DefaultChannels = new() { NotificationChannel.Email, NotificationChannel.Push, NotificationChannel.InApp }
            },
            new NotificationTemplate
            {
                TemplateName = "TREASURY_APPROVAL",
                Type = NotificationType.Treasury,
                SubjectTemplate = "Treasury Approval Required: {{amount}} {{currency}}",
                BodyTemplate = "A treasury operation of {{amount}} {{currency}} requires your approval. Operation: {{operationId}}",
                DefaultChannels = new() { NotificationChannel.Email, NotificationChannel.Push, NotificationChannel.InApp }
            },
            new NotificationTemplate
            {
                TemplateName = "KYC_STATUS",
                Type = NotificationType.Update,
                SubjectTemplate = "KYC Verification {{status}}",
                BodyTemplate = "Your KYC verification has been {{status}}. {{message}}",
                DefaultChannels = new() { NotificationChannel.Email, NotificationChannel.InApp }
            },
            new NotificationTemplate
            {
                TemplateName = "MEMBERSHIP_UPGRADE",
                Type = NotificationType.Info,
                SubjectTemplate = "Membership Upgraded to {{tier}}!",
                BodyTemplate = "Congratulations! Your membership has been upgraded to {{tier}}. Enjoy {{profitShare}}% profit sharing.",
                DefaultChannels = new() { NotificationChannel.Email, NotificationChannel.Push, NotificationChannel.InApp }
            }
        };

        foreach (var t in templates)
            _templates[t.TemplateName] = t;
    }

    public async Task<Notification> SendAsync(SendNotificationRequest request)
    {
        var id = Interlocked.Increment(ref _counter);
        var notification = new Notification
        {
            NotificationId = $"NOTIF-{id}",
            Type = request.Type,
            Priority = request.Priority,
            Title = request.Title,
            Body = request.Body,
            RichContent = request.RichContent,
            ActionUrl = request.ActionUrl,
            Data = request.Data,
            RecipientIds = request.RecipientIds,
            RecipientEmails = request.RecipientEmails,
            Channels = request.Channels ?? new() { NotificationChannel.InApp },
            SenderId = request.SenderId,
            SenderName = request.SenderName,
            SagaId = request.SagaId,
            ScheduledAt = request.ScheduledAt
        };

        // Process each channel
        foreach (var channel in notification.Channels)
        {
            foreach (var recipientId in notification.RecipientIds)
            {
                var delivery = new NotificationDelivery
                {
                    NotificationId = notification.Id,
                    RecipientId = recipientId,
                    Channel = channel
                };

                // Simulate sending
                delivery.Status = DeliveryStatus.Sent;
                delivery.DeliveredAt = DateTime.UtcNow;

                if (!_deliveries.ContainsKey(notification.Id))
                    _deliveries[notification.Id] = new();
                _deliveries[notification.Id].Add(delivery);
            }
        }

        notification.Status = NotificationStatus.Sent;
        notification.SentAt = DateTime.UtcNow;
        _notifications[notification.Id] = notification;

        return await Task.FromResult(notification);
    }

    public async Task<Notification> SendFromTemplateAsync(string templateName, Dictionary<string, string> variables, List<string> recipientIds)
    {
        var template = await GetTemplateAsync(templateName);
        if (template == null)
            throw new ArgumentException($"Template '{templateName}' not found");

        var title = template.SubjectTemplate;
        var body = template.BodyTemplate;

        foreach (var (key, value) in variables)
        {
            title = title.Replace($"{{{{{key}}}}}", value);
            body = body.Replace($"{{{{{key}}}}}", value);
        }

        return await SendAsync(new SendNotificationRequest
        {
            Type = template.Type,
            Title = title,
            Body = body,
            RecipientIds = recipientIds,
            Channels = template.DefaultChannels
        });
    }

    public Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        // In production, use MailKit to send actual emails
        Console.WriteLine($"[EMAIL] To: {to}, Subject: {subject}");
        return Task.FromResult(true);
    }

    public Task<bool> SendSmsAsync(string to, string message)
    {
        // In production, integrate with Twilio/SMS provider
        Console.WriteLine($"[SMS] To: {to}, Message: {message}");
        return Task.FromResult(true);
    }

    public Task<bool> SendPushAsync(string token, string title, string body, Dictionary<string, string>? data = null)
    {
        // In production, use Firebase Admin SDK
        Console.WriteLine($"[PUSH] Token: {token[..20]}..., Title: {title}");
        return Task.FromResult(true);
    }

    public Task<Notification?> GetAsync(Guid id)
    {
        _notifications.TryGetValue(id, out var notification);
        return Task.FromResult(notification);
    }

    public Task<IReadOnlyList<Notification>> GetForUserAsync(string userId, int limit = 50)
    {
        var notifications = _notifications.Values
            .Where(n => n.RecipientIds.Contains(userId))
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .ToList();
        return Task.FromResult<IReadOnlyList<Notification>>(notifications);
    }

    public Task<bool> MarkAsReadAsync(Guid id, string userId)
    {
        if (_deliveries.TryGetValue(id, out var deliveries))
        {
            var delivery = deliveries.FirstOrDefault(d => d.RecipientId == userId);
            if (delivery != null)
            {
                delivery.IsRead = true;
                delivery.ReadAt = DateTime.UtcNow;
                delivery.Status = DeliveryStatus.Read;
                return Task.FromResult(true);
            }
        }
        return Task.FromResult(false);
    }

    public Task<bool> MarkAllAsReadAsync(string userId)
    {
        foreach (var deliveries in _deliveries.Values)
        {
            foreach (var d in deliveries.Where(d => d.RecipientId == userId && !d.IsRead))
            {
                d.IsRead = true;
                d.ReadAt = DateTime.UtcNow;
                d.Status = DeliveryStatus.Read;
            }
        }
        return Task.FromResult(true);
    }

    public Task<NotificationTemplate> CreateTemplateAsync(NotificationTemplate template)
    {
        _templates[template.TemplateName] = template;
        return Task.FromResult(template);
    }

    public Task<NotificationTemplate?> GetTemplateAsync(string templateName)
    {
        _templates.TryGetValue(templateName, out var template);
        return Task.FromResult(template);
    }

    public Task<UserPreferences> GetPreferencesAsync(string userId)
    {
        if (!_preferences.TryGetValue(userId, out var prefs))
        {
            prefs = new UserPreferences { UserId = userId };
            _preferences[userId] = prefs;
        }
        return Task.FromResult(prefs);
    }

    public Task<UserPreferences> UpdatePreferencesAsync(string userId, UserPreferences preferences)
    {
        preferences.UserId = userId;
        _preferences[userId] = preferences;
        return Task.FromResult(preferences);
    }
}

public record SendNotificationRequest
{
    public NotificationType Type { get; init; }
    public NotificationPriority Priority { get; init; } = NotificationPriority.Normal;
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string? RichContent { get; init; }
    public string? ActionUrl { get; init; }
    public Dictionary<string, string>? Data { get; init; }
    public List<string> RecipientIds { get; init; } = new();
    public List<string>? RecipientEmails { get; init; }
    public List<NotificationChannel>? Channels { get; init; }
    public string? SenderId { get; init; }
    public string? SenderName { get; init; }
    public string? SagaId { get; init; }
    public DateTime? ScheduledAt { get; init; }
}
