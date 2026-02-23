using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.Blockchain.Rbac;

/// <summary>
/// Extension methods for dependency injection of RbacClient.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRbacClient(
        this IServiceCollection services,
        Action<RbacClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<RbacClientOptions>(_ => { });
        }

        services.AddSingleton<IRbacClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RbacClientOptions>>();
            var logger = sp.GetService<ILogger<RbacClient>>();
            return new RbacClient(options, logger);
        });

        return services;
    }

    public static IServiceCollection AddRbacClient(
        this IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfigurationSection configurationSection)
    {
        services.Configure<RbacClientOptions>(configurationSection);

        services.AddSingleton<IRbacClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RbacClientOptions>>();
            var logger = sp.GetService<ILogger<RbacClient>>();
            return new RbacClient(options, logger);
        });

        return services;
    }
}

