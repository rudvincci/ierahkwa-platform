using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mamey.FWID.Identities.Infrastructure.Redis.Options;
using Mamey.FWID.Identities.Infrastructure.Redis.Repositories;
using Mamey.FWID.Identities.Infrastructure.Redis.Services;
using Mamey.Microservice.Infrastructure;

namespace Mamey.FWID.Identities.Infrastructure.Redis;

internal static class Extensions
{
    public static IMameyBuilder AddRedisRepositories(this IMameyBuilder builder, string sectionName = "redisSync")
    {
        // Register Redis sync options
        builder.Services.Configure<RedisSyncOptions>(builder.Configuration.GetSection(sectionName));

        // ICache is already registered by AddRedis() called elsewhere
        // Register Redis repositories for composite repos
        builder.Services.AddScoped<IdentityRedisRepository>();
        
        // Register Postgres repositories needed by composite repos
        builder.Services.AddScoped<IdentityPostgresRepository>();
        
        // Register background sync service to sync from Postgres to Redis
        builder.Services.AddHostedService<IdentityRedisSyncService>();
        
        return builder;
    }
}

