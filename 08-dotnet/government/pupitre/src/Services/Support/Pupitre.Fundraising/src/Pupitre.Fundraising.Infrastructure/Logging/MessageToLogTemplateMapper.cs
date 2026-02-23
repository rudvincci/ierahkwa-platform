using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Fundraising.Application.Commands;
using Pupitre.Fundraising.Application.Exceptions;
using Pupitre.Fundraising.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddCampaign),     
                new HandlerLogTemplate
                {
                    After = "Added a campaign with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(CampaignAlreadyExistsException), "Campaign with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteCampaign),  new HandlerLogTemplate { After = "Deleted a campaign with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}