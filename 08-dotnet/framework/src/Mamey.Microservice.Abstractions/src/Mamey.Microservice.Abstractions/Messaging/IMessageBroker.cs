using Mamey.CQRS.Events;
using Mamey.MessageBrokers;

namespace Mamey.Microservice.Abstractions.Messaging
{
    public interface IMessageBroker
    {
        Task PublishAsync(params IEvent[] events);
        Task PublishAsync(IEnumerable<IEvent> events);
        Task PublishAsync(IMessage message, CancellationToken cancellationToken = default);
        Task PublishAsync(IMessage[] messages, CancellationToken cancellationToken = default);
    }
}

