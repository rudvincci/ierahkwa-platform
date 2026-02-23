using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.AIAssessments.Application.Commands;
using Pupitre.AIAssessments.Application.Exceptions;
using Pupitre.AIAssessments.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddAIAssessment),     
                new HandlerLogTemplate
                {
                    After = "Added a aiassessment with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(AIAssessmentAlreadyExistsException), "AIAssessment with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteAIAssessment),  new HandlerLogTemplate { After = "Deleted a aiassessment with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}