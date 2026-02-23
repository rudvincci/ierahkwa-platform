using Mamey.MessageBrokers.Outbox.Messages;

namespace Mamey.MessageBrokers.Outbox;

public interface IMessageOutboxAccessor
{
    Task<IReadOnlyList<OutboxMessage>> GetUnsentAsync();
    Task ProcessAsync(OutboxMessage message);
    Task ProcessAsync(IEnumerable<OutboxMessage> outboxMessages);
}