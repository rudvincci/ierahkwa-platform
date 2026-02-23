using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Compliance.Application.Commands;
using Pupitre.Compliance.Application.Exceptions;
using Pupitre.Compliance.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddComplianceRecord),     
                new HandlerLogTemplate
                {
                    After = "Added a compliancerecord with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(ComplianceRecordAlreadyExistsException), "ComplianceRecord with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteComplianceRecord),  new HandlerLogTemplate { After = "Deleted a compliancerecord with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}