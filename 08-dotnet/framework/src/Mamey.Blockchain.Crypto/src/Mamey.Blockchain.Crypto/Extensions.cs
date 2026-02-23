using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.Crypto;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add MameyCryptoClient to the service collection
    /// </summary>
    public static IServiceCollection AddMameyCryptoClient(
        this IServiceCollection services,
        Action<MameyCryptoClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<MameyCryptoClientOptions>(_ => { });
        }

        services.AddSingleton<MameyCryptoClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MameyCryptoClientOptions>>();
            var logger = sp.GetService<ILogger<MameyCryptoClient>>();
            return new MameyCryptoClient(options, logger);
        });

        return services;
    }
}
