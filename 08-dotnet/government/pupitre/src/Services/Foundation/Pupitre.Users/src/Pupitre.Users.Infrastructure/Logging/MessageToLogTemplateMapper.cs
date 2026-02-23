using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Pupitre.Users.Application.Commands;
using Pupitre.Users.Application.Exceptions;
using Pupitre.Users.Contracts.Commands;

namespace Mamey.Microservice.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddUser),     
                new HandlerLogTemplate
                {
                    After = "Added a user with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(UserAlreadyExistsException), "User with id: {Id} already exists."},
                    }
                }
            },
            {typeof(DeleteUser),  new HandlerLogTemplate { After = "Deleted a user with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}