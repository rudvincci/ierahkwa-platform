namespace Mamey.MessageBrokers.RabbitMQ;

public interface IConventionsProvider
{
    IConventions Get<T>();
    IConventions Get(Type type);
}