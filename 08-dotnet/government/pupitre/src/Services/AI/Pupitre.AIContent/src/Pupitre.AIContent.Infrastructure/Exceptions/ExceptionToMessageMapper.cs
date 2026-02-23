using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.AIContent.Application.Events.Rejected;
using Pupitre.AIContent.Application.Exceptions;
using System;
using Pupitre.AIContent.Domain.Exceptions;
using Pupitre.AIContent.Application.Commands;
using Mamey.Exceptions;
using Pupitre.AIContent.Contracts.Commands;

namespace Pupitre.AIContent.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            ContentGenerationAlreadyExistsException ex => message switch
            {
                AddContentGeneration cmd => new AddContentGenerationRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            ContentGenerationNotFoundException ex => message switch
            {
                UpdateContentGeneration cmd => new UpdateContentGenerationRejected(ex.ContentGenerationId, ex.Message, ex.Code),
                DeleteContentGeneration cmd => new DeleteContentGenerationRejected(ex.ContentGenerationId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

