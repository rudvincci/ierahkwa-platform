namespace Mamey.Microservice.Abstractions.Streaming
{
    public interface IStreamSubscriber
    {
        Task SubscribAsync<T>(string topic, Action<T> handler) where T : class;
    }
}

