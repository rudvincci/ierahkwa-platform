namespace Mamey.MessageBrokers.RabbitMQ.Plugins;

internal interface IRabbitMqPluginsRegistryAccessor
{
    LinkedList<RabbitMqPluginChain> Get();
}