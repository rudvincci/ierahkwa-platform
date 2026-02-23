using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.Wallet;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add MameyWalletClient to the service collection
    /// </summary>
    public static IServiceCollection AddMameyWalletClient(
        this IServiceCollection services,
        Action<MameyWalletClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<MameyWalletClientOptions>(_ => { });
        }

        services.AddSingleton<MameyWalletClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MameyWalletClientOptions>>();
            var logger = sp.GetService<ILogger<MameyWalletClient>>();
            return new MameyWalletClient(options, logger);
        });

        return services;
    }
}
