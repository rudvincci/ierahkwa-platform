using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AI.Core.Embeddings;
using Pupitre.AI.Core.Options;
using Pupitre.AI.Core.Orchestration;

namespace Pupitre.AI.Core;

/// <summary>
/// Extension methods for registering AI services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds Pupitre AI core services to the service collection.
    /// </summary>
    public static IServiceCollection AddPupitreAI(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(AIOptions.SectionName);
        services.Configure<AIOptions>(opts =>
        {
            section.Bind(opts);
        });

        services.AddSingleton<ILLMRouter, LLMRouter>();
        services.AddSingleton<IEmbeddingService, EmbeddingService>();

        return services;
    }
}
