using System.ComponentModel.DataAnnotations;
using Mamey.Government.Modules.Notifications.Core.Domain.Types;
using Mamey.Types;

namespace Mamey.Government.Modules.Notifications.Core.Domain.Entities;

    internal class Notification : AggregateRoot<NotificationId>
    {
        private Notification(){}

        public Notification(NotificationId id, string icon, string title, string message,
            NotificationCategory category, DateTime timestamp, UserId? userId = null, int version = 0)
            : base(id, version)
        {
            Id = Id;
            UserId = userId;
            Icon = icon;
            Title = title;
            Message = message;
            Timestamp = timestamp;
            Category = category;
        }

        public UserId? UserId { get; private set; }
        [MaxLength(30)]
        public string Icon { get; private set; } = string.Empty;
        [MaxLength(50)]
        public string Title { get; private set; } = string.Empty;
        [MaxLength(300)]
        public string? Message { get; private set; } = string.Empty;
        public DateTime Timestamp { get; private set; }
        public bool IsRead => ReadAt.HasValue;
        public DateTime? ReadAt { get; private set; }
        public NotificationCategory Category { get; private set; } = NotificationCategory.General;

        public virtual void SetRead()
        {
            ReadAt = DateTime.UtcNow;
        }
    }
    public enum NotificationCategory
    {
        General = 0,                      // General notifications
        SystemAlert = 1,                  // System-related alerts
        ChatMessage = 2,                  // Chat or message notifications
        TaskUpdate = 3,                   // Updates about tasks or assignments
        Reminder = 4,                     // Reminder notifications
        Promotion = 5,                    // Promotional or marketing notifications
        Error = 6,                        // Error or issue-related notifications
        Warning = 7,                      // Warning notifications
        Success = 8,                      // Success or confirmation notifications
        Security = 9,                     // Security-related notifications
        // Government-specific categories
        ApplicationStatusChanged = 10,    // Citizenship application status changed
        DocumentIssued = 11,              // Passport, ID card, or certificate issued
        DocumentReady = 12,               // Document ready for download
        PaymentDue = 13,                  // Payment due reminder
        PaymentReceived = 14,             // Payment received confirmation
        StatusProgression = 15,           // Citizenship status progression (Probationary -> Resident -> Citizen)
        DocumentExpiring = 16,            // Document expiration warning
        ReviewRequired = 17,               // Agent review required
        KycCompleted = 18,                // KYC verification completed
        KycFailed = 19,                   // KYC verification failed
        Custom = 99                       // Custom or uncategorized notifications
    }