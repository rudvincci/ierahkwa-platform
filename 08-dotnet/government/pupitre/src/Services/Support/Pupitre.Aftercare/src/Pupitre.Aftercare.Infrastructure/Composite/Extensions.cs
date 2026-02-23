using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Aftercare.Domain.Repositories;

namespace Pupitre.Aftercare.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IAftercarePlanRepository, CompositeAftercarePlanRepository>();
        return builder;
    }
}
