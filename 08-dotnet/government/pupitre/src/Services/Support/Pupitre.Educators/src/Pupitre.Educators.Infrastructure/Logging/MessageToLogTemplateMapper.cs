using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Educators.Application.Commands;
using Pupitre.Educators.Application.Exceptions;
using Pupitre.Educators.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddEducator),     
                new HandlerLogTemplate
                {
                    After = "Added a educator with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(EducatorAlreadyExistsException), "Educator with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteEducator),  new HandlerLogTemplate { After = "Deleted a educator with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}