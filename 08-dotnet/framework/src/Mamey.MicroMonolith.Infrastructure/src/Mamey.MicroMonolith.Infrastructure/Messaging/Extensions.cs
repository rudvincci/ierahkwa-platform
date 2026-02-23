using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.MicroMonolith.Infrastructure.Messaging.Brokers;
using Mamey.MicroMonolith.Infrastructure.Messaging.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using IMessageContextRegistry = Mamey.MicroMonolith.Infrastructure.Messaging.Contexts.IMessageContextRegistry;
using MessageContextProvider = Mamey.MicroMonolith.Infrastructure.Messaging.Contexts.MessageContextProvider;
using MessageContextRegistry = Mamey.MicroMonolith.Infrastructure.Messaging.Contexts.MessageContextRegistry;

namespace Mamey.MicroMonolith.Infrastructure.Messaging;

public static class Extensions
{
    private const string SectionName = "messaging";
        
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddTransient<IMessageBroker, InMemoryMessageBroker>();
        services.AddTransient<IAsyncMessageDispatcher, AsyncMessageDispatcher>();
        services.AddSingleton<IMessageChannel, MessageChannel>();
        services.AddSingleton<IMessageContextProvider, MessageContextProvider>();
        services.AddSingleton<IMessageContextRegistry, MessageContextRegistry>();

        var messagingOptions = services.GetOptions<MessagingOptions>(SectionName);
        services.AddSingleton(messagingOptions);

        if (messagingOptions.UseAsyncDispatcher)
        {
            services.AddHostedService<AsyncDispatcherJob>();
        }
            
        return services;
    }
}