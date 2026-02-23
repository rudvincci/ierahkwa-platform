namespace Mamey.MessageBrokers.Outbox;

public interface IMessageOutboxConfigurator
{
    IMameyBuilder Builder { get; }
    OutboxOptions Options { get; }
}