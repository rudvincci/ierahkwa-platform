using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.AIRecommendations.Application.Commands;
using Pupitre.AIRecommendations.Application.Exceptions;
using Pupitre.AIRecommendations.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddAIRecommendation),     
                new HandlerLogTemplate
                {
                    After = "Added a airecommendation with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(AIRecommendationAlreadyExistsException), "AIRecommendation with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteAIRecommendation),  new HandlerLogTemplate { After = "Deleted a airecommendation with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}