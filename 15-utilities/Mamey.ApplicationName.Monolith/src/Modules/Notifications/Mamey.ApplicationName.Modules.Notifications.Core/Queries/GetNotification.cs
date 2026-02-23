using Mamey.ApplicationName.Modules.Notifications.Core.DTO;
using Mamey.CQRS.Queries;


namespace Mamey.ApplicationName.Modules.Notifications.Core.Queries;

public class GetNotification : IQuery<NotificationDetailsDto>
{
    public Guid Id { get; set; }
}