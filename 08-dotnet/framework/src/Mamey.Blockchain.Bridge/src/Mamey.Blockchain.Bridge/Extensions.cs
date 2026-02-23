using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.Bridge;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add MameyBridgeClient to the service collection
    /// </summary>
    public static IServiceCollection AddMameyBridgeClient(
        this IServiceCollection services,
        Action<MameyBridgeClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<MameyBridgeClientOptions>(_ => { });
        }

        services.AddSingleton<MameyBridgeClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MameyBridgeClientOptions>>();
            var logger = sp.GetService<ILogger<MameyBridgeClient>>();
            return new MameyBridgeClient(options, logger);
        });

        return services;
    }
}
