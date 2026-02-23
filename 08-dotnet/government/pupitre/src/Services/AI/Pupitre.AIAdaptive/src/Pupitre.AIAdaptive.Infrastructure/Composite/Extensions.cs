using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIAdaptive.Domain.Repositories;

namespace Pupitre.AIAdaptive.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IAdaptiveLearningRepository, CompositeAdaptiveLearningRepository>();
        return builder;
    }
}
