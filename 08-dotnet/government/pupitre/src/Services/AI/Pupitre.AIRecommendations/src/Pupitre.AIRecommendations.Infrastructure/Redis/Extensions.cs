using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIRecommendations.Infrastructure.Redis.Repositories;
using Pupitre.AIRecommendations.Infrastructure.Redis.Services;

namespace Pupitre.AIRecommendations.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<AIRecommendationRedisRepository>();
        builder.Services.AddHostedService<AIRecommendationRedisSyncService>();
        return builder;
    }
}
