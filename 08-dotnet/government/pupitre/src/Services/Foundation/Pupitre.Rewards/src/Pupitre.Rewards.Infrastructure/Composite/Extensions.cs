using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Rewards.Domain.Repositories;

namespace Pupitre.Rewards.Infrastructure.Composite;

internal static class Extensions
{
    public static IMameyBuilder AddCompositeRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<IRewardRepository, CompositeRewardRepository>();
        return builder;
    }
}
