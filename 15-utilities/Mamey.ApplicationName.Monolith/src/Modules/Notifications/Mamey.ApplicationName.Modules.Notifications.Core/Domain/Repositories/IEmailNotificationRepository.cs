using Mamey.ApplicationName.Modules.Notifications.Core.Domain.Entities;

namespace Mamey.ApplicationName.Modules.Notifications.Core.Domain.Repositories;

internal interface IEmailNotificationRepository
{
    Task<EmailNotification?> GetAsync(Guid id);
    Task AddAsync(EmailNotification notification);
    Task UpdateAsync(EmailNotification notification);
}