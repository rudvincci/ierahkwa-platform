using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIVision.Infrastructure.Redis.Repositories;
using Pupitre.AIVision.Infrastructure.Redis.Services;

namespace Pupitre.AIVision.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<VisionAnalysisRedisRepository>();
        builder.Services.AddHostedService<VisionAnalysisRedisSyncService>();
        return builder;
    }
}
