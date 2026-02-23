using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.GLEs.Domain.Repositories;

namespace Pupitre.GLEs.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IGLERepository, CompositeGLERepository>();
        return builder;
    }
}
