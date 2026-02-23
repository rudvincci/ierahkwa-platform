using RabbitMQ.Client.Events;

namespace Mamey.MessageBrokers.RabbitMQ;

public interface IRabbitMqPlugin
{
    Task HandleAsync(object message, object correlationContext, BasicDeliverEventArgs args);
}