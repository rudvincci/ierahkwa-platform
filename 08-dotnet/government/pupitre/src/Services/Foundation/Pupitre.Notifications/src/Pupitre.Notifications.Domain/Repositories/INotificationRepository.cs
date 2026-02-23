using System;
using Pupitre.Notifications.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Notifications.Domain.Repositories;

internal interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
    Task DeleteAsync(NotificationId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Notification> GetAsync(NotificationId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(NotificationId id, CancellationToken cancellationToken = default);
}
