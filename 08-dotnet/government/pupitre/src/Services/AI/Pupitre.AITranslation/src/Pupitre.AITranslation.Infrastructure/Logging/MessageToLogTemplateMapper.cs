using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.AITranslation.Application.Commands;
using Pupitre.AITranslation.Application.Exceptions;
using Pupitre.AITranslation.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddTranslationRequest),     
                new HandlerLogTemplate
                {
                    After = "Added a translationrequest with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(TranslationRequestAlreadyExistsException), "TranslationRequest with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteTranslationRequest),  new HandlerLogTemplate { After = "Deleted a translationrequest with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}