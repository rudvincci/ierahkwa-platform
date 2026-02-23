using Mamey.Auth.Decentralized.Core;
using Microsoft.Extensions.Options;

namespace Mamey.Auth.Decentralized.Caching;

/// <summary>
/// Extension methods for DID Document caching
/// </summary>
public static class CacheExtensions
{
    /// <summary>
    /// Adds in-memory DID Document cache
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure cache options</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddInMemoryDidDocumentCache(this IServiceCollection services, Action<CacheOptions>? configureOptions = null)
    {
        var options = new CacheOptions();
        configureOptions?.Invoke(options);

        services.Configure<CacheOptions>(opt =>
        {
            opt.InMemoryExpiration = options.InMemoryExpiration;
            opt.RedisExpiration = options.RedisExpiration;
            opt.MaxInMemoryItems = options.MaxInMemoryItems;
            opt.EnableCacheWarming = options.EnableCacheWarming;
            opt.CacheWarmingBatchSize = options.CacheWarmingBatchSize;
        });

        services.AddSingleton<IDidDocumentCache, InMemoryDidDocumentCache>();
        return services;
    }

    /// <summary>
    /// Adds Redis DID Document cache
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure cache options</param>
    /// <param name="keyPrefix">The key prefix for Redis keys</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddRedisDidDocumentCache(this IServiceCollection services, Action<CacheOptions>? configureOptions = null, string keyPrefix = "did:document:")
    {
        var options = new CacheOptions();
        configureOptions?.Invoke(options);

        services.Configure<CacheOptions>(opt =>
        {
            opt.InMemoryExpiration = options.InMemoryExpiration;
            opt.RedisExpiration = options.RedisExpiration;
            opt.MaxInMemoryItems = options.MaxInMemoryItems;
            opt.EnableCacheWarming = options.EnableCacheWarming;
            opt.CacheWarmingBatchSize = options.CacheWarmingBatchSize;
        });

        services.AddScoped<IDidDocumentCache>(provider =>
        {
            var database = provider.GetRequiredService<IDatabase>();
            var logger = provider.GetRequiredService<ILogger<RedisDidDocumentCache>>();
            return new RedisDidDocumentCache(database, logger, keyPrefix);
        });

        return services;
    }

    /// <summary>
    /// Adds hybrid DID Document cache (in-memory + Redis)
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configureOptions">Action to configure cache options</param>
    /// <param name="keyPrefix">The key prefix for Redis keys</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddHybridDidDocumentCache(this IServiceCollection services, Action<CacheOptions>? configureOptions = null, string keyPrefix = "did:document:")
    {
        var options = new CacheOptions();
        configureOptions?.Invoke(options);

        services.Configure<CacheOptions>(opt =>
        {
            opt.InMemoryExpiration = options.InMemoryExpiration;
            opt.RedisExpiration = options.RedisExpiration;
            opt.MaxInMemoryItems = options.MaxInMemoryItems;
            opt.EnableCacheWarming = options.EnableCacheWarming;
            opt.CacheWarmingBatchSize = options.CacheWarmingBatchSize;
        });

        services.AddSingleton<InMemoryDidDocumentCache>();
        services.AddScoped<RedisDidDocumentCache>(provider =>
        {
            var database = provider.GetRequiredService<IDatabase>();
            var logger = provider.GetRequiredService<ILogger<RedisDidDocumentCache>>();
            return new RedisDidDocumentCache(database, logger, keyPrefix);
        });

        services.AddScoped<IDidDocumentCache>(provider =>
        {
            var inMemoryCache = provider.GetRequiredService<InMemoryDidDocumentCache>();
            var redisCache = provider.GetRequiredService<RedisDidDocumentCache>();
            var logger = provider.GetRequiredService<ILogger<HybridDidDocumentCache>>();
            var options = provider.GetRequiredService<IOptions<CacheOptions>>().Value;
            return new HybridDidDocumentCache(inMemoryCache, redisCache, logger, options);
        });

        return services;
    }

    /// <summary>
    /// Adds DID Document cache with Mamey builder
    /// </summary>
    /// <param name="builder">The Mamey builder</param>
    /// <param name="cacheType">The cache type to use</param>
    /// <param name="configureOptions">Action to configure cache options</param>
    /// <param name="keyPrefix">The key prefix for Redis keys</param>
    /// <returns>The Mamey builder</returns>
    public static IMameyBuilder AddDidDocumentCache(this IMameyBuilder builder, CacheType cacheType = CacheType.Hybrid, Action<CacheOptions>? configureOptions = null, string keyPrefix = "did:document:")
    {
        switch (cacheType)
        {
            case CacheType.InMemory:
                builder.Services.AddInMemoryDidDocumentCache(configureOptions);
                break;
            case CacheType.Redis:
                builder.Services.AddRedisDidDocumentCache(configureOptions, keyPrefix);
                break;
            case CacheType.Hybrid:
                builder.Services.AddHybridDidDocumentCache(configureOptions, keyPrefix);
                break;
            default:
                throw new ArgumentException($"Unsupported cache type: {cacheType}", nameof(cacheType));
        }

        return builder;
    }
}

/// <summary>
/// Cache type enumeration
/// </summary>
public enum CacheType
{
    /// <summary>
    /// In-memory cache only
    /// </summary>
    InMemory,

    /// <summary>
    /// Redis cache only
    /// </summary>
    Redis,

    /// <summary>
    /// Hybrid cache (in-memory + Redis)
    /// </summary>
    Hybrid
}
