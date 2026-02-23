namespace Mamey.MessageBrokers.RabbitMQ;

public interface IRabbitMqClient
{
    Task SendAsync(object message, IConventions conventions, string messageId = null, string correlationId = null,
        string spanContext = null, object messageContext = null, IDictionary<string, object> headers = null);
}