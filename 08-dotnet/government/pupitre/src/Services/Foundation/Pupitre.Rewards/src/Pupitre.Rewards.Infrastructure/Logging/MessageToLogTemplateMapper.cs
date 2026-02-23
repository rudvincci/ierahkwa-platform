using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Rewards.Application.Commands;
using Pupitre.Rewards.Application.Exceptions;
using Pupitre.Rewards.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddReward),     
                new HandlerLogTemplate
                {
                    After = "Added a reward with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(RewardAlreadyExistsException), "Reward with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteReward),  new HandlerLogTemplate { After = "Deleted a reward with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}