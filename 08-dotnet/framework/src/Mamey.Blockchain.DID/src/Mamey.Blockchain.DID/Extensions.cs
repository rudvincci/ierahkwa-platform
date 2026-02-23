using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.DID;

/// <summary>
/// Extension methods for dependency injection of DIDClient.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the DIDClient to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration action.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddDIDClient(
        this IServiceCollection services,
        Action<DIDClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<DIDClientOptions>(_ => { });
        }

        services.AddSingleton<IDIDClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<DIDClientOptions>>();
            var logger = sp.GetService<ILogger<DIDClient>>();
            return new DIDClient(options, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds the DIDClient to the service collection with configuration from appsettings.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configurationSection">The configuration section to bind.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddDIDClient(
        this IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfigurationSection configurationSection)
    {
        services.Configure<DIDClientOptions>(configurationSection);

        services.AddSingleton<IDIDClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<DIDClientOptions>>();
            var logger = sp.GetService<ILogger<DIDClient>>();
            return new DIDClient(options, logger);
        });

        return services;
    }
}
