using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.AITutors.Application.Events.Rejected;
using Pupitre.AITutors.Application.Exceptions;
using System;
using Pupitre.AITutors.Domain.Exceptions;
using Pupitre.AITutors.Application.Commands;
using Mamey.Exceptions;
using Pupitre.AITutors.Contracts.Commands;

namespace Pupitre.AITutors.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            TutorAlreadyExistsException ex => message switch
            {
                AddTutor cmd => new AddTutorRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            TutorNotFoundException ex => message switch
            {
                UpdateTutor cmd => new UpdateTutorRejected(ex.TutorId, ex.Message, ex.Code),
                DeleteTutor cmd => new DeleteTutorRejected(ex.TutorId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

