using Mamey.MessageBrokers;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.MicroMonolith.Infrastructure.Messaging.Contexts;

public interface IMessageContextRegistry
{
    void Set(IMessage message, IMessageContext context);
}