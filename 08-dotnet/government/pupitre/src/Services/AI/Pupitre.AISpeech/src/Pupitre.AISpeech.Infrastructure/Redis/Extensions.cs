using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AISpeech.Infrastructure.Redis.Repositories;
using Pupitre.AISpeech.Infrastructure.Redis.Services;

namespace Pupitre.AISpeech.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<SpeechRequestRedisRepository>();
        builder.Services.AddHostedService<SpeechRequestRedisSyncService>();
        return builder;
    }
}
