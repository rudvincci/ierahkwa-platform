using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.CryptoExchange;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add MameyCryptoExchangeClient to the service collection
    /// </summary>
    public static IServiceCollection AddMameyCryptoExchangeClient(
        this IServiceCollection services,
        Action<MameyCryptoExchangeClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<MameyCryptoExchangeClientOptions>(_ => { });
        }

        services.AddSingleton<MameyCryptoExchangeClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MameyCryptoExchangeClientOptions>>();
            var logger = sp.GetService<ILogger<MameyCryptoExchangeClient>>();
            return new MameyCryptoExchangeClient(options, logger);
        });

        return services;
    }
}
