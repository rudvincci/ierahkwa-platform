using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Curricula.Application.Commands;
using Pupitre.Curricula.Application.Exceptions;
using Pupitre.Curricula.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddCurriculum),     
                new HandlerLogTemplate
                {
                    After = "Added a curriculum with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(CurriculumAlreadyExistsException), "Curriculum with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteCurriculum),  new HandlerLogTemplate { After = "Deleted a curriculum with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}