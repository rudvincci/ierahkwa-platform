using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.GLEs.Application.Events.Rejected;
using Pupitre.GLEs.Application.Exceptions;
using System;
using Pupitre.GLEs.Domain.Exceptions;
using Pupitre.GLEs.Application.Commands;
using Mamey.Exceptions;
using Pupitre.GLEs.Contracts.Commands;

namespace Pupitre.GLEs.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            GLEAlreadyExistsException ex => message switch
            {
                AddGLE cmd => new AddGLERejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            GLENotFoundException ex => message switch
            {
                UpdateGLE cmd => new UpdateGLERejected(ex.GLEId, ex.Message, ex.Code),
                DeleteGLE cmd => new DeleteGLERejected(ex.GLEId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

