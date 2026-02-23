using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Mamey.Blockchain.Banking;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add MameyBankingClient to the service collection
    /// </summary>
    public static IServiceCollection AddMameyBankingClient(
        this IServiceCollection services,
        Action<MameyBankingClientOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<MameyBankingClientOptions>(_ => { });
        }

        services.AddSingleton<MameyBankingClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MameyBankingClientOptions>>();
            var logger = sp.GetService<ILogger<MameyBankingClient>>();
            return new MameyBankingClient(options, logger);
        });
        services.AddSingleton<IMameyBankingClient>(sp => sp.GetRequiredService<MameyBankingClient>());

        return services;
    }
}
