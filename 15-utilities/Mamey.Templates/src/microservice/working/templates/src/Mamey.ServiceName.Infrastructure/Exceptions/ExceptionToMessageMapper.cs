using System;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.ServiceName.Application.Events.Rejected;
using Mamey.ServiceName.Application.Exceptions;
using System;
using Mamey.ServiceName.Domain.Exceptions;
using Mamey.ServiceName.Application.Commands;
using Mamey.Exceptions;
using Mamey.ServiceName.Contracts.Commands;

namespace Mamey.ServiceName.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object? Map(Exception exception, object message)
        => exception switch
        {
            EntityNameAlreadyExistsException ex => message switch
            {
                AddEntityName cmd => new AddEntityNameRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null
            },
            EntityNameNotFoundException ex => message switch
            {
                UpdateEntityName cmd => new UpdateEntityNameRejected(ex.EntityNameId, ex.Message, ex.Code),
                DeleteEntityName cmd => new DeleteEntityNameRejected(ex.EntityNameId, ex.Message, ex.Code),
                _ => null
            },
            _ => null
        };
}

