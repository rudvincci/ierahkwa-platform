using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.AIBehavior.Application.Commands;
using Pupitre.AIBehavior.Application.Exceptions;
using Pupitre.AIBehavior.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddBehavior),     
                new HandlerLogTemplate
                {
                    After = "Added a behavior with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(BehaviorAlreadyExistsException), "Behavior with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteBehavior),  new HandlerLogTemplate { After = "Deleted a behavior with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}