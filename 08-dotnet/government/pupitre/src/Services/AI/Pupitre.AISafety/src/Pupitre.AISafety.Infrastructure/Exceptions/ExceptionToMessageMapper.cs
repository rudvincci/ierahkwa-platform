using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.AISafety.Application.Events.Rejected;
using Pupitre.AISafety.Application.Exceptions;
using System;
using Pupitre.AISafety.Domain.Exceptions;
using Pupitre.AISafety.Application.Commands;
using Mamey.Exceptions;
using Pupitre.AISafety.Contracts.Commands;

namespace Pupitre.AISafety.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            SafetyCheckAlreadyExistsException ex => message switch
            {
                AddSafetyCheck cmd => new AddSafetyCheckRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            SafetyCheckNotFoundException ex => message switch
            {
                UpdateSafetyCheck cmd => new UpdateSafetyCheckRejected(ex.SafetyCheckId, ex.Message, ex.Code),
                DeleteSafetyCheck cmd => new DeleteSafetyCheckRejected(ex.SafetyCheckId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

