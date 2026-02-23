using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Government.Modules.Notifications.Core.Domain.Types;

namespace Mamey.Government.Modules.Notifications.Core.Domain.Repositories;
    internal interface INotificationRepository
    {
        Task<Notification?> GetAsync(NotificationId id, CancellationToken cancellationToken = default);
        Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
        Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
        Task DeleteAsync(NotificationId id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(NotificationId id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Notification>> BrowseAsync(CancellationToken cancellationToken = default);
    }