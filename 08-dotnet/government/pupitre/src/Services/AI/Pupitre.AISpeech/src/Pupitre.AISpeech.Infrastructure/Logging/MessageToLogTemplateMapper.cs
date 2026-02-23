using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.AISpeech.Application.Commands;
using Pupitre.AISpeech.Application.Exceptions;
using Pupitre.AISpeech.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddSpeechRequest),     
                new HandlerLogTemplate
                {
                    After = "Added a speechrequest with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(SpeechRequestAlreadyExistsException), "SpeechRequest with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteSpeechRequest),  new HandlerLogTemplate { After = "Deleted a speechrequest with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}