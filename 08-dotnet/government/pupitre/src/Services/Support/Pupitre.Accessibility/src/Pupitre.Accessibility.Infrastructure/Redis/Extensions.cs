using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Accessibility.Infrastructure.Redis.Repositories;
using Pupitre.Accessibility.Infrastructure.Redis.Services;

namespace Pupitre.Accessibility.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<AccessProfileRedisRepository>();
        builder.Services.AddHostedService<AccessProfileRedisSyncService>();
        return builder;
    }
}
