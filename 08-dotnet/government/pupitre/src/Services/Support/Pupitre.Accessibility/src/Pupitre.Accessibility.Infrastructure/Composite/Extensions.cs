using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Accessibility.Domain.Repositories;

namespace Pupitre.Accessibility.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IAccessProfileRepository, CompositeAccessProfileRepository>();
        return builder;
    }
}
