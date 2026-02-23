using System.Linq;
using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Government.Modules.Notifications.Core.Domain.Repositories;
using Mamey.Government.Modules.Notifications.Core.Domain.Types;
using Mamey.Government.Shared.Abstractions;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Notifications.Core.EF;

internal class NotificationsInitializer : IInitializer
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationsInitializer> _logger;

    private static readonly string[] NotificationTitles = {
        "Application Status Update", "Document Ready", "Payment Received", "Payment Due",
        "Status Progression", "Document Issued", "Reminder", "System Alert", "Security Alert",
        "Welcome Message", "Account Created", "Profile Updated", "Document Expiring"
    };

    private static readonly string[] NotificationMessages = {
        "Your citizenship application status has been updated.",
        "Your document is ready for download.",
        "Your payment has been successfully processed.",
        "A payment is due for your application.",
        "Congratulations! Your citizenship status has progressed.",
        "A new document has been issued to you.",
        "This is a reminder about your pending application.",
        "System maintenance scheduled for tonight.",
        "Security alert: New login detected.",
        "Welcome to the government services portal!",
        "Your account has been successfully created.",
        "Your profile information has been updated.",
        "One of your documents will expire soon."
    };

    private static readonly string[] Icons = {
        "📋", "📄", "💳", "⏰", "🎉", "🆔", "🔔", "⚠️", "🔒", "👋", "✅", "✏️", "⏳"
    };

    public NotificationsInitializer(
        INotificationRepository notificationRepository,
        ILogger<NotificationsInitializer> logger)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting notifications database initialization...");

        // Check if data already exists
        var existingNotifications = Enumerable.Empty<Notification>();
        try
        {
            existingNotifications = await _notificationRepository.BrowseAsync(cancellationToken);
        }
        catch (Exception ex) when (ex.Message.Contains("does not exist") || 
                                    ex.Message.Contains("relation") || 
                                    ex.Message.Contains("Invalid object name") ||
                                    ex.Message.Contains("Table") && ex.Message.Contains("doesn't exist") ||
                                    ex.Message.Contains("Unknown table"))
        {
            // Table doesn't exist yet (migrations may still be running) - treat as no data
            _logger.LogDebug("Table does not exist yet for Notifications. Treating as empty database.");
            existingNotifications = Enumerable.Empty<Notification>();
        }

        if (existingNotifications.Any())
        {
            _logger.LogInformation("Database already contains {Count} notifications. Skipping seed.", 
                existingNotifications.Count());
            return;
        }

        var random = new Random(42); // Fixed seed for reproducible data
        var notifications = new List<Notification>();

        for (int i = 0; i < 100; i++)
        {
            var notificationId = new NotificationId(SeedData.GenerateDeterministicGuid(i, "notification"));
            var index = random.Next(NotificationTitles.Length);
            var icon = Icons[index];
            var title = NotificationTitles[index];
            var message = NotificationMessages[index];
            var category = (NotificationCategory)random.Next(0, 17); // 0-16 for valid categories
            var timestamp = DateTime.UtcNow.AddDays(-random.Next(30)); // Last 30 days
            // 80% reference actual user IDs (1-50), 20% are system notifications
            var userId = random.Next(100) < 80 ? new UserId(SeedData.GetUserId(random.Next(1, 51))) : null;
            
            var notification = new Notification(
                notificationId,
                icon,
                title,
                message,
                category,
                timestamp,
                userId);
            
            // Mark 30% as read
            if (random.Next(100) < 30)
            {
                notification.SetRead();
            }

            notifications.Add(notification);
        }

        _logger.LogInformation("Created {Count} mock notifications", notifications.Count);

        // Add notifications using repository
        foreach (var notification in notifications)
        {
            await _notificationRepository.AddAsync(notification, cancellationToken);
        }
        
        _logger.LogInformation("Successfully seeded {Count} notifications", notifications.Count);
    }
}
