using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Ministries.Application.Commands;
using Pupitre.Ministries.Application.Exceptions;
using Pupitre.Ministries.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddMinistryData),     
                new HandlerLogTemplate
                {
                    After = "Added a ministrydata with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(MinistryDataAlreadyExistsException), "MinistryData with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteMinistryData),  new HandlerLogTemplate { After = "Deleted a ministrydata with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}