namespace Mamey.MessageBrokers.Outbox.Configurators;

internal sealed class MessageOutboxConfigurator : IMessageOutboxConfigurator
{
    public IMameyBuilder Builder { get; }
    public OutboxOptions Options { get; }

    public MessageOutboxConfigurator(IMameyBuilder builder, OutboxOptions options)
    {
        Builder = builder;
        Options = options;
    }
}