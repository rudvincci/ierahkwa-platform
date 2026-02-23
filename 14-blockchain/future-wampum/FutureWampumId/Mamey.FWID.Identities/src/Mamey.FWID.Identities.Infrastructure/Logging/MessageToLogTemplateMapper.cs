using System;
using System.Collections.Generic;
using Mamey.Logging.CQRS;
using Mamey.FWID.Identities.Application.Commands;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Contracts.Commands;

namespace Mamey.FWID.Identities.Infrastructure.Logging;

internal sealed class MessageToLogTemplateMapper : IMessageToLogTemplateMapper
{
    private static IReadOnlyDictionary<Type, HandlerLogTemplate> MessageTemplates 
        => new Dictionary<Type, HandlerLogTemplate>
        {
            {
                typeof(AddIdentity),     
                new HandlerLogTemplate
                {
                    After = "Added a identity with id: {Id}. ",
                    OnError = new Dictionary<Type, string>
                    {
                        { typeof(IdentityAlreadyExistsException), "Identity with id: {Id} already exists."},
                    }
                }
            },
            {typeof(RevokeIdentity),  new HandlerLogTemplate { After = "Revoked identity with id: {IdentityId}."}},
            {typeof(UpdateZone),  new HandlerLogTemplate { After = "Updated zone for identity with id: {IdentityId}."}},
            {typeof(UpdateContactInformation),  new HandlerLogTemplate { After = "Updated contact information for identity with id: {IdentityId}."}},
        };
    
    public HandlerLogTemplate Map<TMessage>(TMessage message) where TMessage : class
    {
        var key = message.GetType();
        return MessageTemplates.TryGetValue(key, out var template) ? template : null!;
    }
}