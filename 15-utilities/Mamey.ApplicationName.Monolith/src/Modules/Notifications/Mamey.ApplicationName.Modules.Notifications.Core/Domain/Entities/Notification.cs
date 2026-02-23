using System.ComponentModel.DataAnnotations;
using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Types;
using Mamey.Types;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;

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
        General = 0,          // General notifications
        SystemAlert = 1,      // System-related alerts
        ChatMessage = 2,      // Chat or message notifications
        TaskUpdate = 3,       // Updates about tasks or assignments
        Reminder = 4,         // Reminder notifications
        Promotion = 5,        // Promotional or marketing notifications
        Error = 6,            // Error or issue-related notifications
        Warning = 7,          // Warning notifications
        Success = 8,          // Success or confirmation notifications
        Security = 9,         // Security-related notifications
        Custom = 99           // Custom or uncategorized notifications
    }