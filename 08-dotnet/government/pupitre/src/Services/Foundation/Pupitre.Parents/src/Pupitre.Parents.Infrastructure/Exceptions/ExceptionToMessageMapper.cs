using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Parents.Application.Events.Rejected;
using Pupitre.Parents.Application.Exceptions;
using System;
using Pupitre.Parents.Domain.Exceptions;
using Pupitre.Parents.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Parents.Contracts.Commands;

namespace Pupitre.Parents.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            ParentAlreadyExistsException ex => message switch
            {
                AddParent cmd => new AddParentRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            ParentNotFoundException ex => message switch
            {
                UpdateParent cmd => new UpdateParentRejected(ex.ParentId, ex.Message, ex.Code),
                DeleteParent cmd => new DeleteParentRejected(ex.ParentId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

