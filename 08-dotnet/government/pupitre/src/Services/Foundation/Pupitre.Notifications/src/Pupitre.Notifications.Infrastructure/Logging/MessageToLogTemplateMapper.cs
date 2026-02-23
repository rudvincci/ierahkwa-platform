using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Notifications.Application.Commands;
using Pupitre.Notifications.Application.Exceptions;
using Pupitre.Notifications.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddNotification),     
                new HandlerLogTemplate
                {
                    After = "Added a notification with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(NotificationAlreadyExistsException), "Notification with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteNotification),  new HandlerLogTemplate { After = "Deleted a notification with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}