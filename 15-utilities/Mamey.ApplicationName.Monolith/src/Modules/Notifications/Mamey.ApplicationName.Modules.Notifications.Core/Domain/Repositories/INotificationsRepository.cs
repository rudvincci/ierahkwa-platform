using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;
using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Types;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Domain.Repositories;
    internal interface INotificationRepository
    {
        Task<Notification?> GetAsync(NotificationId id);
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
    }