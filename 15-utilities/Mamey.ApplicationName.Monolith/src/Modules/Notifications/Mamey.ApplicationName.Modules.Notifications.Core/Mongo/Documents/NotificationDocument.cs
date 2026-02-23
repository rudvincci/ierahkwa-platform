using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;
using Mamey.ApplicationName.Modules.Notifications.Core.DTO;
using Mamey.MicroMonolith.Infrastructure.Mongo;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Mongo.Documents;
    internal class NotificationDocument : IIdentifiable<Guid>
    {
        public NotificationDocument()
        {
        }

        public NotificationDocument(Notification notification)
        {
            Id = notification.Id.Value;
            UserId = notification.UserId?.Value;
            Icon = notification.Icon;
            Title = notification.Title;
            Message = notification.Message;
            Timestamp = notification.Timestamp.ToUnixTimeMilliseconds();
            IsRead = notification.IsRead;
            Category = notification.Category.ToString();
        }

        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public long Timestamp { get; set; }
        public bool IsRead { get; set; }
        public string Category { get; set; }
        
        
        public Notification AsEntity()
            => new Notification(Id, Icon, Title, Message, Category.ToEnum<NotificationCategory>(),Timestamp.GetDate(), UserId);
        public NotificationDto AsDto()
            => new NotificationDto
            {
                Id = Id,
                Icon = Icon,
                Title = Title,
                UserId = UserId.ToString(),
            };
        public NotificationDetailsDto AsDetailsDto()
            => new NotificationDetailsDto(Id, Icon, Title, Message, Category, Timestamp.GetDate(), IsRead, UserId.ToString());
    }