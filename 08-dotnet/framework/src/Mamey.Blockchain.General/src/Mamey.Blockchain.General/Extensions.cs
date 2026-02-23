using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.General;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add MameyGeneralClient to the service collection
    /// </summary>
    public static IServiceCollection AddMameyGeneralClient(
        this IServiceCollection services,
        Action<MameyGeneralClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<MameyGeneralClientOptions>(_ => { });
        }

        services.AddSingleton<MameyGeneralClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MameyGeneralClientOptions>>();
            var logger = sp.GetService<ILogger<MameyGeneralClient>>();
            return new MameyGeneralClient(options, logger);
        });

        return services;
    }
}
