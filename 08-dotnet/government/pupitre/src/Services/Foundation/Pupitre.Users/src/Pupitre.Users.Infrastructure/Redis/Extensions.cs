using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Users.Infrastructure.Redis.Repositories;
using Pupitre.Users.Infrastructure.Redis.Services;

namespace Pupitre.Users.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<UserRedisRepository>();
        builder.Services.AddHostedService<UserRedisSyncService>();
        return builder;
    }
}
