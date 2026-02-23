using Mamey.Microservice.Abstractions.Streaming;
using Mamey.Microservice.Infrastructure.Serialization;
using StackExchange.Redis;

namespace Mamey.Microservice.Infrastructure.Redis.Streaming
{
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
            => _subscriber.SubscribeAsync(RedisChannel.Literal(topic), (_, data) =>
            {
                try
                {
                    if (data.IsNullOrEmpty)
                    {
                        return;
                    }
                    var payload = _serializer.Deserialize<T>(data!);
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
}

