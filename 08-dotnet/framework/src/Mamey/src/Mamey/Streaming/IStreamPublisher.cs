namespace Mamey.Streaming;

public interface IStreamPublisher
{
    Task PublishAsync<T>(string topic, T data) where T : class;
}