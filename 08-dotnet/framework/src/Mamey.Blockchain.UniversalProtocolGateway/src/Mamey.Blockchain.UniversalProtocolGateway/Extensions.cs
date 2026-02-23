using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.UniversalProtocolGateway;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add MameyUPGClient to the service collection
    /// </summary>
    public static IServiceCollection AddMameyUPGClient(
        this IServiceCollection services,
        Action<MameyUPGClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<MameyUPGClientOptions>(_ => { });
        }

        services.AddSingleton<MameyUPGClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MameyUPGClientOptions>>();
            var logger = sp.GetService<ILogger<MameyUPGClient>>();
            return new MameyUPGClient(options, logger);
        });

        return services;
    }
}
