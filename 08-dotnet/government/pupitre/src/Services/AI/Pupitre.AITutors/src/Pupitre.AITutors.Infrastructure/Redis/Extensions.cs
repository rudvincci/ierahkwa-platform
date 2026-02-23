using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AITutors.Infrastructure.Redis.Repositories;
using Pupitre.AITutors.Infrastructure.Redis.Services;

namespace Pupitre.AITutors.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<TutorRedisRepository>();
        builder.Services.AddHostedService<TutorRedisSyncService>();
        return builder;
    }
}
