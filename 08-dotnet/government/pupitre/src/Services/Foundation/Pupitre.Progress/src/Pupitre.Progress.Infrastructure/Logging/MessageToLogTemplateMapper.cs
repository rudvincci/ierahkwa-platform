using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Progress.Application.Commands;
using Pupitre.Progress.Application.Exceptions;
using Pupitre.Progress.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddLearningProgress),     
                new HandlerLogTemplate
                {
                    After = "Added a learningprogress with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(LearningProgressAlreadyExistsException), "LearningProgress with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteLearningProgress),  new HandlerLogTemplate { After = "Deleted a learningprogress with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}