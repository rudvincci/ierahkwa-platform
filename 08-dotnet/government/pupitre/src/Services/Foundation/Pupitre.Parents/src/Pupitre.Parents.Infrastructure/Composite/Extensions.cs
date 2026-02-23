using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Parents.Domain.Repositories;

namespace Pupitre.Parents.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IParentRepository, CompositeParentRepository>();
        return builder;
    }
}
