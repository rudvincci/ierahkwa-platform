using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIContent.Domain.Repositories;

namespace Pupitre.AIContent.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IContentGenerationRepository, CompositeContentGenerationRepository>();
        return builder;
    }
}
