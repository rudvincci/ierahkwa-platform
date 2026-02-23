using Mamey.Microservice.Abstractions.Streaming;

namespace Mamey.Microservice.Infrastructure.Streaming
{
    internal sealed class DefaultStreamPublisher : IStreamPublisher
    {
        public DefaultStreamPublisher()
        {
        }

        public Task PublishAsync<T>(string topic, T data) where T : class
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class DefaultStreamSubscriber : IStreamSubscriber
    {
        public DefaultStreamSubscriber()
        {

        }

        public async Task SubscribAsync<T>(string topic, Action<T> handler) where T : class
        {
            throw new NotImplementedException();
        }
    }

    internal static class Extensions
    {
        public static IServiceCollection AddStreaming(this IServiceCollection services)
            => services
                .AddSingleton<IStreamPublisher, DefaultStreamPublisher>()
                .AddSingleton<IStreamSubscriber, DefaultStreamSubscriber>();
               
                
    }
}

