using Mamey.CQRS.Events;
using Mamey.FWID.Notifications.Application.Events.Rejected;
using Mamey.FWID.Notifications.Domain.Exceptions;
using Mamey.MessageBrokers;

namespace Mamey.FWID.Notifications.Infrastructure.Exceptions;

internal class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public IRejectedEvent? Map(Exception exception, object message)
        => exception switch
        {
            NotificationAlreadyReadException ex => new MarkAsReadRejected(ex.NotificationId.Value, ex.NotificationId.IdentityId.Value, ex.Message, "notification_already_read"),
            _ => null
        };
}







