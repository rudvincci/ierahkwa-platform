using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.Compliance;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add MameyComplianceClient to the service collection
    /// </summary>
    public static IServiceCollection AddMameyComplianceClient(
        this IServiceCollection services,
        Action<MameyComplianceClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<MameyComplianceClientOptions>(_ => { });
        }

        services.AddSingleton<MameyComplianceClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MameyComplianceClientOptions>>();
            var logger = sp.GetService<ILogger<MameyComplianceClient>>();
            return new MameyComplianceClient(options, logger);
        });

        return services;
    }
}
