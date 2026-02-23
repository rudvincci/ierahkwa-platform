using System.Threading.Tasks;
using Mamey.MessageBrokers;

namespace Mamey.MicroMonolith.Infrastructure.Messaging.Outbox;

public interface IOutboxBroker
{
    bool Enabled { get; }
    Task SendAsync(params IMessage[] messages);
}