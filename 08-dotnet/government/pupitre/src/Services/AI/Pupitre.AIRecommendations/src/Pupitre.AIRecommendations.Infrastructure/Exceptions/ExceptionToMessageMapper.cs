using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.AIRecommendations.Application.Events.Rejected;
using Pupitre.AIRecommendations.Application.Exceptions;
using System;
using Pupitre.AIRecommendations.Domain.Exceptions;
using Pupitre.AIRecommendations.Application.Commands;
using Mamey.Exceptions;
using Pupitre.AIRecommendations.Contracts.Commands;

namespace Pupitre.AIRecommendations.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            AIRecommendationAlreadyExistsException ex => message switch
            {
                AddAIRecommendation cmd => new AddAIRecommendationRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            AIRecommendationNotFoundException ex => message switch
            {
                UpdateAIRecommendation cmd => new UpdateAIRecommendationRejected(ex.AIRecommendationId, ex.Message, ex.Code),
                DeleteAIRecommendation cmd => new DeleteAIRecommendationRejected(ex.AIRecommendationId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

