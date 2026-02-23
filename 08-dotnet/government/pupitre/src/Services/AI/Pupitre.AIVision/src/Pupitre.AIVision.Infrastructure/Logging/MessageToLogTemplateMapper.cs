using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.AIVision.Application.Commands;
using Pupitre.AIVision.Application.Exceptions;
using Pupitre.AIVision.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddVisionAnalysis),     
                new HandlerLogTemplate
                {
                    After = "Added a visionanalysis with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(VisionAnalysisAlreadyExistsException), "VisionAnalysis with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteVisionAnalysis),  new HandlerLogTemplate { After = "Deleted a visionanalysis with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}