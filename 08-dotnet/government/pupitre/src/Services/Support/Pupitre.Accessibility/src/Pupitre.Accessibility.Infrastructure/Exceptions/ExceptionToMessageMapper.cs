using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Accessibility.Application.Events.Rejected;
using Pupitre.Accessibility.Application.Exceptions;
using System;
using Pupitre.Accessibility.Domain.Exceptions;
using Pupitre.Accessibility.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Accessibility.Contracts.Commands;

namespace Pupitre.Accessibility.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            AccessProfileAlreadyExistsException ex => message switch
            {
                AddAccessProfile cmd => new AddAccessProfileRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            AccessProfileNotFoundException ex => message switch
            {
                UpdateAccessProfile cmd => new UpdateAccessProfileRejected(ex.AccessProfileId, ex.Message, ex.Code),
                DeleteAccessProfile cmd => new DeleteAccessProfileRejected(ex.AccessProfileId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

