using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Accessibility.Application.Commands;
using Pupitre.Accessibility.Application.Exceptions;
using Pupitre.Accessibility.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddAccessProfile),     
                new HandlerLogTemplate
                {
                    After = "Added a accessprofile with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(AccessProfileAlreadyExistsException), "AccessProfile with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteAccessProfile),  new HandlerLogTemplate { After = "Deleted a accessprofile with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}