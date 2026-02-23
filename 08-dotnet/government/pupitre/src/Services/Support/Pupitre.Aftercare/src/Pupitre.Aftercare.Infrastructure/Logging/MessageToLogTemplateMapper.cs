using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Aftercare.Application.Commands;
using Pupitre.Aftercare.Application.Exceptions;
using Pupitre.Aftercare.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddAftercarePlan),     
                new HandlerLogTemplate
                {
                    After = "Added a aftercareplan with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(AftercarePlanAlreadyExistsException), "AftercarePlan with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteAftercarePlan),  new HandlerLogTemplate { After = "Deleted a aftercareplan with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}