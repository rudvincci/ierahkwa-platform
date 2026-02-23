using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIAdaptive.Infrastructure.Redis.Repositories;
using Pupitre.AIAdaptive.Infrastructure.Redis.Services;

namespace Pupitre.AIAdaptive.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<AdaptiveLearningRedisRepository>();
        builder.Services.AddHostedService<AdaptiveLearningRedisSyncService>();
        return builder;
    }
}
