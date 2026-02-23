using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Parents.Application.Commands;
using Pupitre.Parents.Application.Exceptions;
using Pupitre.Parents.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddParent),     
                new HandlerLogTemplate
                {
                    After = "Added a parent with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(ParentAlreadyExistsException), "Parent with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteParent),  new HandlerLogTemplate { After = "Deleted a parent with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}