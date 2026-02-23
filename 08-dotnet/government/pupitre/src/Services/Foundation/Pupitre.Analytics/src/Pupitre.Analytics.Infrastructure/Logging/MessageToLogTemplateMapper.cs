using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Analytics.Application.Commands;
using Pupitre.Analytics.Application.Exceptions;
using Pupitre.Analytics.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddAnalytic),     
                new HandlerLogTemplate
                {
                    After = "Added a analytic with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(AnalyticAlreadyExistsException), "Analytic with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteAnalytic),  new HandlerLogTemplate { After = "Deleted a analytic with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}