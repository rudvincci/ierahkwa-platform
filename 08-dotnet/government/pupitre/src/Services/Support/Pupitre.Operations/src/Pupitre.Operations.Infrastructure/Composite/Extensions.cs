using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Operations.Domain.Repositories;

namespace Pupitre.Operations.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IOperationMetricRepository, CompositeOperationMetricRepository>();
        return builder;
    }
}
