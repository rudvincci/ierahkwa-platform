using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.Advanced;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add MameyAdvancedClient to the service collection
    /// </summary>
    public static IServiceCollection AddMameyAdvancedClient(
        this IServiceCollection services,
        Action<MameyAdvancedClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<MameyAdvancedClientOptions>(_ => { });
        }

        services.AddSingleton<MameyAdvancedClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MameyAdvancedClientOptions>>();
            var logger = sp.GetService<ILogger<MameyAdvancedClient>>();
            return new MameyAdvancedClient(options, logger);
        });

        return services;
    }
}
