using Mamey;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.GLEs.Infrastructure.Redis.Repositories;
using Pupitre.GLEs.Infrastructure.Redis.Services;

namespace Pupitre.GLEs.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder)
    {
        builder.Services.AddScoped<GLERedisRepository>();
        builder.Services.AddHostedService<GLERedisSyncService>();
        return builder;
    }
}
