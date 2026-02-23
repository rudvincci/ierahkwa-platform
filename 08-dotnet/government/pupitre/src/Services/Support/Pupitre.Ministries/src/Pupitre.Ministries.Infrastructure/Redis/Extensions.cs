using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Ministries.Infrastructure.Redis.Repositories;
using Pupitre.Ministries.Infrastructure.Redis.Services;

namespace Pupitre.Ministries.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<MinistryDataRedisRepository>();
        builder.Services.AddHostedService<MinistryDataRedisSyncService>();
        return builder;
    }
}
