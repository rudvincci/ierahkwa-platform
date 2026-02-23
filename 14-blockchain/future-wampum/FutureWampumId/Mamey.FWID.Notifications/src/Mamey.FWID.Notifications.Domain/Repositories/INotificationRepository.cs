using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Entities;

namespace Mamey.FWID.Notifications.Domain.Repositories;

/// <summary>
/// Repository interface for notifications.
/// </summary>
internal interface INotificationRepository
{
    Task<Notification?> GetAsync(NotificationId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetByIdentityIdAsync(IdentityId identityId, CancellationToken cancellationToken = default);
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
    Task DeleteAsync(NotificationId id, CancellationToken cancellationToken = default);
}







