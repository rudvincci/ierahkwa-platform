using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Notifications.Application.Events.Rejected;
using Pupitre.Notifications.Application.Exceptions;
using System;
using Pupitre.Notifications.Domain.Exceptions;
using Pupitre.Notifications.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Notifications.Contracts.Commands;

namespace Pupitre.Notifications.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            NotificationAlreadyExistsException ex => message switch
            {
                AddNotification cmd => new AddNotificationRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            NotificationNotFoundException ex => message switch
            {
                UpdateNotification cmd => new UpdateNotificationRejected(ex.NotificationId, ex.Message, ex.Code),
                DeleteNotification cmd => new DeleteNotificationRejected(ex.NotificationId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

