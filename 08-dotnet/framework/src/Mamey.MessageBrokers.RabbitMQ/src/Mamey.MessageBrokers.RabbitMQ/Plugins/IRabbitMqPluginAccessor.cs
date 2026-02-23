using RabbitMQ.Client.Events;

namespace Mamey.MessageBrokers.RabbitMQ.Plugins;

internal interface IRabbitMqPluginAccessor
{
    void SetSuccessor(Func<object, object, BasicDeliverEventArgs, Task> successor);
}