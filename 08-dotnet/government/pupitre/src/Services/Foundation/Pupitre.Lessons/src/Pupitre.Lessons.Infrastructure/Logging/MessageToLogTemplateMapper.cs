using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Lessons.Application.Commands;
using Pupitre.Lessons.Application.Exceptions;
using Pupitre.Lessons.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddLesson),     
                new HandlerLogTemplate
                {
                    After = "Added a lesson with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(LessonAlreadyExistsException), "Lesson with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteLesson),  new HandlerLogTemplate { After = "Deleted a lesson with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}