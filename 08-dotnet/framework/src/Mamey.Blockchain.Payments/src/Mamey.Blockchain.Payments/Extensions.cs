using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.Payments;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add MameyPaymentClient to the service collection
    /// </summary>
    public static IServiceCollection AddMameyPaymentClient(
        this IServiceCollection services,
        Action<MameyPaymentClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<MameyPaymentClientOptions>(_ => { });
        }

        services.AddSingleton<MameyPaymentClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MameyPaymentClientOptions>>();
            var logger = sp.GetService<ILogger<MameyPaymentClient>>();
            return new MameyPaymentClient(options, logger);
        });

        return services;
    }
}
