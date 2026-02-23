using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Aftercare.Infrastructure.Redis.Repositories;
using Pupitre.Aftercare.Infrastructure.Redis.Services;

namespace Pupitre.Aftercare.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<AftercarePlanRedisRepository>();
        builder.Services.AddHostedService<AftercarePlanRedisSyncService>();
        return builder;
    }
}
