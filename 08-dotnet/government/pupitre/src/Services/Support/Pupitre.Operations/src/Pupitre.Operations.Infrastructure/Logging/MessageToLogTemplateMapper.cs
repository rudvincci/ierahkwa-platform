using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Operations.Application.Commands;
using Pupitre.Operations.Application.Exceptions;
using Pupitre.Operations.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddOperationMetric),     
                new HandlerLogTemplate
                {
                    After = "Added a operationmetric with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(OperationMetricAlreadyExistsException), "OperationMetric with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteOperationMetric),  new HandlerLogTemplate { After = "Deleted a operationmetric with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}