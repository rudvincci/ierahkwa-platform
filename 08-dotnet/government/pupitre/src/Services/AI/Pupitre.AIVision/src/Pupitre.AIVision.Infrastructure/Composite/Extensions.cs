using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIVision.Domain.Repositories;

namespace Pupitre.AIVision.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IVisionAnalysisRepository, CompositeVisionAnalysisRepository>();
        return builder;
    }
}
