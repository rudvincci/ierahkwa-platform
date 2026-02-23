using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Analytics.Infrastructure.Redis.Repositories;
using Pupitre.Analytics.Infrastructure.Redis.Services;

namespace Pupitre.Analytics.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<AnalyticRedisRepository>();
        builder.Services.AddHostedService<AnalyticRedisSyncService>();
        return builder;
    }
}
