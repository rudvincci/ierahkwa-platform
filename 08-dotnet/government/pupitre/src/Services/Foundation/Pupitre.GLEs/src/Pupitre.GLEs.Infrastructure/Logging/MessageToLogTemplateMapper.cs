using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.GLEs.Application.Commands;
using Pupitre.GLEs.Application.Exceptions;
using Pupitre.GLEs.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddGLE),     
                new HandlerLogTemplate
                {
                    After = "Added a gle with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(GLEAlreadyExistsException), "GLE with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteGLE),  new HandlerLogTemplate { After = "Deleted a gle with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}