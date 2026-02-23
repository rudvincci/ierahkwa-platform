using System;
using Mamey.MicroMonolith.Abstractions.Contexts;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.MicroMonolith.Infrastructure.Messaging.Contexts;

public class MessageContext : IMessageContext
{
    public Guid MessageId { get; }
    public IContext Context { get; }

    public MessageContext(Guid messageId, IContext context)
    {
        MessageId = messageId;
        Context = context;
    }
}