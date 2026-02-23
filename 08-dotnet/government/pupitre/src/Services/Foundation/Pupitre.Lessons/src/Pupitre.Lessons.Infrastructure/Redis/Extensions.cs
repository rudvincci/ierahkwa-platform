using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Lessons.Infrastructure.Redis.Repositories;
using Pupitre.Lessons.Infrastructure.Redis.Services;

namespace Pupitre.Lessons.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<LessonRedisRepository>();
        builder.Services.AddHostedService<LessonRedisSyncService>();
        return builder;
    }
}
