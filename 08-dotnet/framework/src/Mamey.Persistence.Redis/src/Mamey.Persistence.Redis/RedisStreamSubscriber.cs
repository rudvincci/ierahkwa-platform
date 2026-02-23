using Mamey.Serialization;
using Mamey.Streaming;
using StackExchange.Redis;

namespace Mamey.Persistence.Redis;

internal sealed class RedisStreamSubscriber : IStreamSubscriber
{
    private readonly ISubscriber _subscriber;
    private readonly ISerializer _serializer;

    public RedisStreamSubscriber(IConnectionMultiplexer connectionMultiplexer, ISerializer serializer)
    {
        _subscriber = connectionMultiplexer.GetSubscriber();
        _serializer = serializer;
    }

    public Task SubscribAsync<T>(string topic, Action<T> handler) where T : class
        => _subscriber.SubscribeAsync(topic, (_, data) =>
        {
            try
            {
                var payload = _serializer.Deserialize<T>(data);
                if (payload is null)
                {
                    return;
                }
                handler(payload);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        });
}