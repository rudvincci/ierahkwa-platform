using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Assessments.Application.Commands;
using Pupitre.Assessments.Application.Exceptions;
using Pupitre.Assessments.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddAssessment),     
                new HandlerLogTemplate
                {
                    After = "Added a assessment with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(AssessmentAlreadyExistsException), "Assessment with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteAssessment),  new HandlerLogTemplate { After = "Deleted a assessment with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}