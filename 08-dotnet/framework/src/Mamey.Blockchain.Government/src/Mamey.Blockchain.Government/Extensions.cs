using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.Government;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add MameyGovernmentClient to the service collection
    /// </summary>
    public static IServiceCollection AddMameyGovernmentClient(
        this IServiceCollection services,
        Action<MameyGovernmentClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<MameyGovernmentClientOptions>(_ => { });
        }

        services.AddSingleton<MameyGovernmentClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MameyGovernmentClientOptions>>();
            var logger = sp.GetService<ILogger<MameyGovernmentClient>>();
            return new MameyGovernmentClient(options, logger);
        });

        services.AddSingleton<IMameyGovernmentClient>(sp =>
            sp.GetRequiredService<MameyGovernmentClient>());

        return services;
    }
}
