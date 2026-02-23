using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Mamey.ServiceName.Application.Commands;
using Mamey.ServiceName.Application.Exceptions;
using Mamey.ServiceName.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddEntityName),     
                new HandlerLogTemplate
                {
                    After = "Added a entityname with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(EntityNameAlreadyExistsException), "EntityName with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteEntityName),  new HandlerLogTemplate { After = "Deleted a entityname with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}