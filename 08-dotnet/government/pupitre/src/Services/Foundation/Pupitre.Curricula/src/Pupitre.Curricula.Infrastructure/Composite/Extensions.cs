using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Curricula.Domain.Repositories;

namespace Pupitre.Curricula.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ICurriculumRepository, CompositeCurriculumRepository>();
        return builder;
    }
}
