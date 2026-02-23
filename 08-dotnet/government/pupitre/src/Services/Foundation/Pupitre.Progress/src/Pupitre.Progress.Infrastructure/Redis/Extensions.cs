using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Progress.Infrastructure.Redis.Repositories;
using Pupitre.Progress.Infrastructure.Redis.Services;

namespace Pupitre.Progress.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<LearningProgressRedisRepository>();
        builder.Services.AddHostedService<LearningProgressRedisSyncService>();
        return builder;
    }
}
