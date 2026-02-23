using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AISafety.Infrastructure.Redis.Repositories;
using Pupitre.AISafety.Infrastructure.Redis.Services;

namespace Pupitre.AISafety.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<SafetyCheckRedisRepository>();
        builder.Services.AddHostedService<SafetyCheckRedisSyncService>();
        return builder;
    }
}
