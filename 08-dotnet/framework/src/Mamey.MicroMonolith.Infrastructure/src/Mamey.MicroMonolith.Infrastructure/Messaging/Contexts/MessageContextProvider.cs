
using Microsoft.Extensions.Caching.Memory;
using Mamey.MessageBrokers;
using Mamey.MicroMonolith.Abstractions.Messaging;

namespace Mamey.MicroMonolith.Infrastructure.Messaging.Contexts;

public class MessageContextProvider : IMessageContextProvider
{
    private readonly IMemoryCache _cache;

    public MessageContextProvider(IMemoryCache cache)
    {
        _cache = cache;
    }

    public IMessageContext Get(IMessage message) => _cache.Get<IMessageContext>(message);
}