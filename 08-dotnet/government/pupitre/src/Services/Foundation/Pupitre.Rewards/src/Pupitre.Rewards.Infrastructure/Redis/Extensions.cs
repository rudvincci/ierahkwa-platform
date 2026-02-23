using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Rewards.Infrastructure.Redis.Repositories;
using Pupitre.Rewards.Infrastructure.Redis.Services;

namespace Pupitre.Rewards.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<RewardRedisRepository>();
        builder.Services.AddHostedService<RewardRedisSyncService>();
        return builder;
    }
}
