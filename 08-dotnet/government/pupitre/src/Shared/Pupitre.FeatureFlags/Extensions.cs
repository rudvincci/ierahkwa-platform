using Microsoft.Extensions.DependencyInjection;

namespace Pupitre.FeatureFlags;

public static class Extensions
{
    /// <summary>
    /// Adds feature flag services to the service collection.
    /// </summary>
    public static IServiceCollection AddPupitreFeatureFlags(this IServiceCollection services)
    {
        services.AddSingleton<IFeatureFlagService, FeatureFlagService>();
        return services;
    }
}
