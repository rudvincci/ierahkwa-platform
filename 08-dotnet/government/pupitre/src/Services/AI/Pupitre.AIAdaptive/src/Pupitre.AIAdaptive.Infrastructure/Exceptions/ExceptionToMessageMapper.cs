using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.AIAdaptive.Application.Events.Rejected;
using Pupitre.AIAdaptive.Application.Exceptions;
using System;
using Pupitre.AIAdaptive.Domain.Exceptions;
using Pupitre.AIAdaptive.Application.Commands;
using Mamey.Exceptions;
using Pupitre.AIAdaptive.Contracts.Commands;

namespace Pupitre.AIAdaptive.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            AdaptiveLearningAlreadyExistsException ex => message switch
            {
                AddAdaptiveLearning cmd => new AddAdaptiveLearningRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            AdaptiveLearningNotFoundException ex => message switch
            {
                UpdateAdaptiveLearning cmd => new UpdateAdaptiveLearningRejected(ex.AdaptiveLearningId, ex.Message, ex.Code),
                DeleteAdaptiveLearning cmd => new DeleteAdaptiveLearningRejected(ex.AdaptiveLearningId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

