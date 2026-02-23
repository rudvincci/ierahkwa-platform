using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.AIContent.Application.Commands;
using Pupitre.AIContent.Application.Exceptions;
using Pupitre.AIContent.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddContentGeneration),     
                new HandlerLogTemplate
                {
                    After = "Added a contentgeneration with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(ContentGenerationAlreadyExistsException), "ContentGeneration with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteContentGeneration),  new HandlerLogTemplate { After = "Deleted a contentgeneration with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}