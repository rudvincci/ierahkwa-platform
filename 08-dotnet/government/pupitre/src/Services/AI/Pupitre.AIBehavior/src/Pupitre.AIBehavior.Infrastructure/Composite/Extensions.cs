using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIBehavior.Domain.Repositories;

namespace Pupitre.AIBehavior.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IBehaviorRepository, CompositeBehaviorRepository>();
        return builder;
    }
}
