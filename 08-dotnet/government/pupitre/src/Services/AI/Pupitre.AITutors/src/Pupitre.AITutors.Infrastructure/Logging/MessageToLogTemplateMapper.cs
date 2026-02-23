using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.AITutors.Application.Commands;
using Pupitre.AITutors.Application.Exceptions;
using Pupitre.AITutors.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddTutor),     
                new HandlerLogTemplate
                {
                    After = "Added a tutor with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(TutorAlreadyExistsException), "Tutor with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteTutor),  new HandlerLogTemplate { After = "Deleted a tutor with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}