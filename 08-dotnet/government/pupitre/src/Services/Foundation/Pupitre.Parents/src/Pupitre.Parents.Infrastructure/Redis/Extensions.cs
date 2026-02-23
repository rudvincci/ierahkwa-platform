using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Parents.Infrastructure.Redis.Repositories;
using Pupitre.Parents.Infrastructure.Redis.Services;

namespace Pupitre.Parents.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ParentRedisRepository>();
        builder.Services.AddHostedService<ParentRedisSyncService>();
        return builder;
    }
}
