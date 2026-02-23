using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AISafety.Domain.Repositories;

namespace Pupitre.AISafety.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ISafetyCheckRepository, CompositeSafetyCheckRepository>();
        return builder;
    }
}
