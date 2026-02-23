using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Mamey.Identity.Redis.Configuration;
using Mamey.Identity.Redis.Services;
using Mamey.Identity.Core;

namespace Mamey.Identity.Redis;

public static class Extensions
{
    public static IServiceCollection AddMameyIdentityRedis(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Mamey:Identity:Redis")
    {
        services.Configure<RedisTokenCacheOptions>(configuration.GetSection(sectionName));
        
        services.AddStackExchangeRedisCache(options =>
        {
            var redisOptions = configuration.GetSection(sectionName).Get<RedisTokenCacheOptions>() ?? new RedisTokenCacheOptions();
            options.Configuration = redisOptions.ConnectionString;
            options.InstanceName = redisOptions.KeyPrefix;
        });

        services.AddSingleton<IRedisTokenCache, Services.RedisTokenCache>();

        return services;
    }

    public static IServiceCollection AddMameyIdentityRedis(
        this IServiceCollection services,
        Action<RedisTokenCacheOptions> configureOptions)
    {
        services.Configure(configureOptions);
        
        services.AddStackExchangeRedisCache(options =>
        {
            var redisOptions = new RedisTokenCacheOptions();
            configureOptions(redisOptions);
            options.Configuration = redisOptions.ConnectionString;
            options.InstanceName = redisOptions.KeyPrefix;
        });

        services.AddSingleton<IRedisTokenCache, Services.RedisTokenCache>();

        return services;
    }

    public static IServiceCollection AddMameyIdentityRedis(
        this IServiceCollection services,
        string connectionString,
        string? keyPrefix = null)
    {
        services.Configure<RedisTokenCacheOptions>(options =>
        {
            options.ConnectionString = connectionString;
            if (!string.IsNullOrEmpty(keyPrefix))
            {
                options.KeyPrefix = keyPrefix;
            }
        });
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = keyPrefix ?? "mamey:identity";
        });

        services.AddSingleton<IRedisTokenCache, Services.RedisTokenCache>();

        return services;
    }
}
