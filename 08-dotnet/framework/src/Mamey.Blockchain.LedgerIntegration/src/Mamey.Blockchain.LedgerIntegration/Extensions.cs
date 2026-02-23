using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.LedgerIntegration;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add MameyLedgerClient to the service collection
    /// </summary>
    public static IServiceCollection AddMameyLedgerClient(
        this IServiceCollection services,
        Action<MameyLedgerClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<MameyLedgerClientOptions>(_ => { });
        }

        services.AddSingleton<MameyLedgerClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MameyLedgerClientOptions>>();
            var logger = sp.GetService<ILogger<MameyLedgerClient>>();
            return new MameyLedgerClient(options, logger);
        });

        return services;
    }
}
