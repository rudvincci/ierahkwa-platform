using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.AIAdaptive.Application.Commands;
using Pupitre.AIAdaptive.Application.Exceptions;
using Pupitre.AIAdaptive.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddAdaptiveLearning),     
                new HandlerLogTemplate
                {
                    After = "Added a adaptivelearning with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(AdaptiveLearningAlreadyExistsException), "AdaptiveLearning with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteAdaptiveLearning),  new HandlerLogTemplate { After = "Deleted a adaptivelearning with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}