using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Progress.Domain.Repositories;

namespace Pupitre.Progress.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ILearningProgressRepository, CompositeLearningProgressRepository>();
        return builder;
    }
}
