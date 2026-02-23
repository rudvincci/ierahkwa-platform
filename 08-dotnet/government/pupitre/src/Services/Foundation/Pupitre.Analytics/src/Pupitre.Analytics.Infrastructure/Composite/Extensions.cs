using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Analytics.Domain.Repositories;

namespace Pupitre.Analytics.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IAnalyticRepository, CompositeAnalyticRepository>();
        return builder;
    }
}
