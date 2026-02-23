using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIRecommendations.Domain.Repositories;

namespace Pupitre.AIRecommendations.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IAIRecommendationRepository, CompositeAIRecommendationRepository>();
        return builder;
    }
}
