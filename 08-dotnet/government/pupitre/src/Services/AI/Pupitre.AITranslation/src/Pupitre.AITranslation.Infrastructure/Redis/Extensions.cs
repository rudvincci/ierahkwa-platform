using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AITranslation.Infrastructure.Redis.Repositories;
using Pupitre.AITranslation.Infrastructure.Redis.Services;

namespace Pupitre.AITranslation.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<TranslationRequestRedisRepository>();
        builder.Services.AddHostedService<TranslationRequestRedisSyncService>();
        return builder;
    }
}
