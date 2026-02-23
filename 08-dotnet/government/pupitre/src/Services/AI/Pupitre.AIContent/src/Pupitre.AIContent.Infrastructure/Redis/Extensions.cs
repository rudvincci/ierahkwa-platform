using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AIContent.Infrastructure.Redis.Repositories;
using Pupitre.AIContent.Infrastructure.Redis.Services;

namespace Pupitre.AIContent.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<ContentGenerationRedisRepository>();
        builder.Services.AddHostedService<ContentGenerationRedisSyncService>();
        return builder;
    }
}
