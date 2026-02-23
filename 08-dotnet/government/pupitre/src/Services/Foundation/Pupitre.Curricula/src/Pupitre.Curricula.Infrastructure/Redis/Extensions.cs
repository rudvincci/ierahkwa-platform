using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.Curricula.Infrastructure.Redis.Repositories;
using Pupitre.Curricula.Infrastructure.Redis.Services;

namespace Pupitre.Curricula.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<CurriculumRedisRepository>();
        builder.Services.AddHostedService<CurriculumRedisSyncService>();
        return builder;
    }
}
