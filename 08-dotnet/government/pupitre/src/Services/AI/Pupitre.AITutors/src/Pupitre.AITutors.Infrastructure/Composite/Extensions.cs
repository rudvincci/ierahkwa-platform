using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AITutors.Domain.Repositories;

namespace Pupitre.AITutors.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ITutorRepository, CompositeTutorRepository>();
        return builder;
    }
}
