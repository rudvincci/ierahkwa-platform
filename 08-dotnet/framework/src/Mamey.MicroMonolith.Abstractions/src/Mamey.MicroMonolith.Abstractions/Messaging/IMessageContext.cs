using Mamey.MicroMonolith.Abstractions.Contexts;

namespace Mamey.MicroMonolith.Abstractions.Messaging;

public interface IMessageContext
{
    public Guid MessageId { get; }
    public IContext Context { get; }
}