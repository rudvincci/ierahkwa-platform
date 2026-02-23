using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Notifications.Infrastructure.Redis.Repositories;
using Pupitre.Notifications.Infrastructure.Redis.Services;

namespace Pupitre.Notifications.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<NotificationRedisRepository>();
        builder.Services.AddHostedService<NotificationRedisSyncService>();
        return builder;
    }
}
