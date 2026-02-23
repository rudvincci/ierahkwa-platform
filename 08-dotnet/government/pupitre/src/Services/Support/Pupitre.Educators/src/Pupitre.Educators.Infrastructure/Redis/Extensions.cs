using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Educators.Infrastructure.Redis.Repositories;
using Pupitre.Educators.Infrastructure.Redis.Services;

namespace Pupitre.Educators.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<EducatorRedisRepository>();
        builder.Services.AddHostedService<EducatorRedisSyncService>();
        return builder;
    }
}
