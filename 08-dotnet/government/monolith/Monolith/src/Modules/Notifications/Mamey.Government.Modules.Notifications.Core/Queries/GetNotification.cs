using Mamey.Government.Modules.Notifications.Core.DTO;
using Mamey.CQRS.Queries;


namespace Mamey.Government.Modules.Notifications.Core.Queries;

public class GetNotification : IQuery<NotificationDetailsDto>
{
    public Guid Id { get; set; }
}