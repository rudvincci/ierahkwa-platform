using Mamey.MessageBrokers.RabbitMQ;
using Mamey.Tracing.Jaeger.RabbitMQ.Plugins;

namespace Mamey.Tracing.Jaeger.RabbitMQ;

public static class Extensions
{
    public static IRabbitMqPluginsRegistry AddJaegerRabbitMqPlugin(this IRabbitMqPluginsRegistry registry)
    {
        registry.Add<JaegerPlugin>();
        return registry;
    }
}