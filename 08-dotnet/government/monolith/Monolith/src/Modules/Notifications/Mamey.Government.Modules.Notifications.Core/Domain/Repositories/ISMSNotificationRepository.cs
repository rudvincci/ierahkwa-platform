using Mamey.Government.Modules.Notifications.Core.Domain.Entities;

namespace Mamey.Government.Modules.Notifications.Core.Domain.Repositories;

internal interface ISMSNotificationRepository
{
    Task<SMSNotification?> GetAsync(Guid id);
    Task AddAsync(SMSNotification notification);
    Task UpdateAsync(SMSNotification notification);
}