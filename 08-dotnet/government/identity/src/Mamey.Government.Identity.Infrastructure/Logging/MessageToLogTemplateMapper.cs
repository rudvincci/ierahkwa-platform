using Mamey.Government.Identity.Contracts.Commands;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Logging.CQRS;

namespace Mamey.Government.Identity.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(CreateUser),     
                new HandlerLogTemplate
                {
                    After = "Added a user with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(UserAlreadyExistsException), "User with id: {Id} already exists."},
                    }
                }
            },
            // {typeof(DeleteUser),  new HandlerLogTemplate { After = "Deleted a user with id: {Id}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null;
    }
}