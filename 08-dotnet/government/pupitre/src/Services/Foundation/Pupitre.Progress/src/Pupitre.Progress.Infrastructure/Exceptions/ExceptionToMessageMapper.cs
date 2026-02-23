using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Progress.Application.Events.Rejected;
using Pupitre.Progress.Application.Exceptions;
using System;
using Pupitre.Progress.Domain.Exceptions;
using Pupitre.Progress.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Progress.Contracts.Commands;

namespace Pupitre.Progress.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            LearningProgressAlreadyExistsException ex => message switch
            {
                AddLearningProgress cmd => new AddLearningProgressRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            LearningProgressNotFoundException ex => message switch
            {
                UpdateLearningProgress cmd => new UpdateLearningProgressRejected(ex.LearningProgressId, ex.Message, ex.Code),
                DeleteLearningProgress cmd => new DeleteLearningProgressRejected(ex.LearningProgressId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

