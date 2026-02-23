namespace Mamey.Streaming;

public interface IStreamSubscriber
{
    Task SubscribAsync<T>(string topic, Action<T> handler) where T : class;
}