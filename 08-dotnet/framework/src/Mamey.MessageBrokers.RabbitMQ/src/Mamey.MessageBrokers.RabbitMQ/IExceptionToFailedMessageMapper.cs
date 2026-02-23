namespace Mamey.MessageBrokers.RabbitMQ;

public interface IExceptionToFailedMessageMapper
{
    FailedMessage Map(Exception exception, object message);
}