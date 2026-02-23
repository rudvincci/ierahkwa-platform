using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Educators.Domain.Repositories;

namespace Pupitre.Educators.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IEducatorRepository, CompositeEducatorRepository>();
        return builder;
    }
}
