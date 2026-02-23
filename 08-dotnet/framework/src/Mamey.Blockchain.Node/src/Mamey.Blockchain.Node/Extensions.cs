using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.Node;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add MameyNodeClient to the service collection
    /// </summary>
    public static IServiceCollection AddMameyNodeClient(
        this IServiceCollection services,
        Action<MameyNodeClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<MameyNodeClientOptions>(_ => { });
        }

        services.AddSingleton<MameyNodeClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MameyNodeClientOptions>>();
            var logger = sp.GetService<ILogger<MameyNodeClient>>();
            return new MameyNodeClient(options, logger);
        });
        services.AddSingleton<IMameyNodeClient>(sp => sp.GetRequiredService<MameyNodeClient>());

        return services;
    }
}
