using Microsoft.Extensions.DependencyInjection;
using Mamey.Blockchain.Node;

namespace Mamey.Blockchain.Swap;

/// <summary>
/// Extension methods for dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Mamey blockchain swap services to the service collection
    /// </summary>
    public static IServiceCollection AddMameyBlockchainSwap(
        this IServiceCollection services)
    {
        services.AddScoped<SwapClient>();
        services.AddScoped<SwapRouter>();
        services.AddScoped<LiquidityPoolClient>();

        return services;
    }
}


























