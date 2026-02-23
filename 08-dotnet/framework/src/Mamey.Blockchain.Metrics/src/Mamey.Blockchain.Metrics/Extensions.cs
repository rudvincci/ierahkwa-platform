using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.Metrics;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add MameyMetricsClient to the service collection
    /// </summary>
    public static IServiceCollection AddMameyMetricsClient(
        this IServiceCollection services,
        Action<MameyMetricsClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<MameyMetricsClientOptions>(_ => { });
        }

        services.AddSingleton<MameyMetricsClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MameyMetricsClientOptions>>();
            var logger = sp.GetService<ILogger<MameyMetricsClient>>();
            return new MameyMetricsClient(options, logger);
        });

        return services;
    }
}
