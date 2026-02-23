using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIBehavior.Infrastructure.Redis.Repositories;
using Pupitre.AIBehavior.Infrastructure.Redis.Services;

namespace Pupitre.AIBehavior.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<BehaviorRedisRepository>();
        builder.Services.AddHostedService<BehaviorRedisSyncService>();
        return builder;
    }
}
