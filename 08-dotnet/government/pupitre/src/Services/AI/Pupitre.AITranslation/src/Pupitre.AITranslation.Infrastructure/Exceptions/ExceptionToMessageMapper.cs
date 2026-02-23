using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.AITranslation.Application.Events.Rejected;
using Pupitre.AITranslation.Application.Exceptions;
using System;
using Pupitre.AITranslation.Domain.Exceptions;
using Pupitre.AITranslation.Application.Commands;
using Mamey.Exceptions;
using Pupitre.AITranslation.Contracts.Commands;

namespace Pupitre.AITranslation.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            TranslationRequestAlreadyExistsException ex => message switch
            {
                AddTranslationRequest cmd => new AddTranslationRequestRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            TranslationRequestNotFoundException ex => message switch
            {
                UpdateTranslationRequest cmd => new UpdateTranslationRequestRejected(ex.TranslationRequestId, ex.Message, ex.Code),
                DeleteTranslationRequest cmd => new DeleteTranslationRequestRejected(ex.TranslationRequestId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

