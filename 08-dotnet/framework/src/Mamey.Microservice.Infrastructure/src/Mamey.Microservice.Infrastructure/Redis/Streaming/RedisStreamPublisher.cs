using Mamey.Microservice.Abstractions.Streaming;
using Mamey.Microservice.Infrastructure.Serialization;
using StackExchange.Redis;

namespace Mamey.Microservice.Infrastructure.Redis.Streaming
{
    internal sealed class RedisStreamPublisher : IStreamPublisher
    {
        private readonly ISubscriber _subscriber;
        private readonly ISerializer _serializer;
        
        public RedisStreamPublisher(IConnectionMultiplexer connectionMultiplexer, ISerializer serializer)
        {
            _subscriber = connectionMultiplexer.GetSubscriber();
            _serializer = serializer;
        }

        public Task PublishAsync<T>(string topic, T data) where T : class
        {
            var payload = _serializer.Serialize(data);
            return _subscriber.PublishAsync(RedisChannel.Literal(topic), payload);
        }
    }

    internal static class Extensions
    {
        public static IServiceCollection AddRedisStreaming(this IServiceCollection services)
            => services
                .AddSingleton<IStreamPublisher, RedisStreamPublisher>()
                .AddSingleton<IStreamSubscriber, RedisStreamSubscriber>();
    }
}

