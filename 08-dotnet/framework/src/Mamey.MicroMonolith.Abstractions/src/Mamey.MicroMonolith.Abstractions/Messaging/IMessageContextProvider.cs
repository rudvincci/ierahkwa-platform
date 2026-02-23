using Mamey.MessageBrokers;

namespace Mamey.MicroMonolith.Abstractions.Messaging;

public interface IMessageContextProvider
{
    IMessageContext Get(IMessage message);
}