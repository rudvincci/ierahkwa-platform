using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.Lending;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add MameyLendingClient to the service collection
    /// </summary>
    public static IServiceCollection AddMameyLendingClient(
        this IServiceCollection services,
        Action<MameyLendingClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<MameyLendingClientOptions>(_ => { });
        }

        services.AddSingleton<MameyLendingClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MameyLendingClientOptions>>();
            var logger = sp.GetService<ILogger<MameyLendingClient>>();
            return new MameyLendingClient(options, logger);
        });

        return services;
    }
}
