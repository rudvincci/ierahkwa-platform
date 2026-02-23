using Mamey.AI.Identity.Configuration;
using Mamey.AI.Identity.Services;
using Mamey.AI.Identity.ML;
using Mamey;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.AI.Identity;

/// <summary>
/// Extension methods for registering Mamey.AI.Identity services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds Mamey.AI.Identity services to the service collection.
    /// </summary>
    public static IMameyBuilder AddAiIdentity(this IMameyBuilder builder, string sectionName = AiIdentityOptions.SectionName)
    {
        builder.Services.AddAiIdentity(sectionName);
        return builder;
    }

    /// <summary>
    /// Adds Mamey.AI.Identity services to the service collection.
    /// </summary>
    public static IServiceCollection AddAiIdentity(this IServiceCollection services, string sectionName = AiIdentityOptions.SectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = AiIdentityOptions.SectionName;
        }

        // Load configuration
        var options = services.GetOptions<AiIdentityOptions>(sectionName);
        services.AddSingleton(options);

        // Register ML infrastructure
        services.AddSingleton<IModelLoader, ModelLoader>();
        services.AddSingleton<IInferenceEngine, InferenceEngine>();
        services.AddSingleton<IFeatureExtractor, FeatureExtractor>();

        // Register AI services
        services.AddScoped<IFraudDetectionService, FraudDetectionService>();
        services.AddScoped<IBiometricMatchingService, BiometricMatchingService>();
        services.AddScoped<IAnomalyDetectionService, AnomalyDetectionService>();
        services.AddScoped<IRiskScoringService, RiskScoringService>();
        services.AddScoped<IDocumentAnalysisService, DocumentAnalysisService>();

        return services;
    }
}
