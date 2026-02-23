using Mamey.CQRS.Queries;

namespace Mamey.Government.Modules.Notifications.Core.Queries;

internal class GetEmailNotification : IQuery<EmailNotificationDetailsDto>
{
    public Guid Id { get; set; }
}