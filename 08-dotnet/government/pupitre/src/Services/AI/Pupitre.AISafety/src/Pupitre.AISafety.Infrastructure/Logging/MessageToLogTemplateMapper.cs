using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.AISafety.Application.Commands;
using Pupitre.AISafety.Application.Exceptions;
using Pupitre.AISafety.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddSafetyCheck),     
                new HandlerLogTemplate
                {
                    After = "Added a safetycheck with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(SafetyCheckAlreadyExistsException), "SafetyCheck with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteSafetyCheck),  new HandlerLogTemplate { After = "Deleted a safetycheck with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}