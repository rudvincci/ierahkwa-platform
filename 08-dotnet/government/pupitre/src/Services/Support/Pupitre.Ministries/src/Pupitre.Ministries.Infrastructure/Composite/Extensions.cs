using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Ministries.Domain.Repositories;

namespace Pupitre.Ministries.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IMinistryDataRepository, CompositeMinistryDataRepository>();
        return builder;
    }
}
