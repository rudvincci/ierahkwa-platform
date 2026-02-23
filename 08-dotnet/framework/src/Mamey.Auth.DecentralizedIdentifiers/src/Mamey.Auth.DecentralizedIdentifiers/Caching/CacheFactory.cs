using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Persistence.Redis;

namespace Mamey.Auth.DecentralizedIdentifiers.Caching;

/// <summary>
/// Factory for creating appropriate cache implementations based on configuration
/// </summary>
public interface ICacheFactory
{
    /// <summary>
    /// Create DID document cache
    /// </summary>
    /// <param name="options">Cache options</param>
    /// <returns>DID document cache implementation</returns>
    IDidDocumentCache CreateDidDocumentCache(CacheOptions options);

    /// <summary>
    /// Create verification result cache
    /// </summary>
    /// <param name="options">Cache options</param>
    /// <returns>Verification result cache implementation</returns>
    IVerificationResultCache CreateVerificationResultCache(CacheOptions options);

    /// <summary>
    /// Create credential status cache
    /// </summary>
    /// <param name="options">Cache options</param>
    /// <returns>Credential status cache implementation</returns>
    ICredentialStatusCache CreateCredentialStatusCache(CacheOptions options);
}

/// <summary>
/// Default implementation of cache factory
/// </summary>
public class DefaultCacheFactory : ICacheFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DefaultCacheFactory> _logger;

    public DefaultCacheFactory(IServiceProvider serviceProvider, ILogger<DefaultCacheFactory> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IDidDocumentCache CreateDidDocumentCache(CacheOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (!options.Enabled)
        {
            _logger.LogInformation("DID document caching is disabled");
            return new NoOpDidDocumentCache();
        }

        switch (options.StorageType?.ToLowerInvariant())
        {
            case "redis":
                return CreateRedisDidDocumentCache();
            case "memory":
                return CreateMemoryDidDocumentCache();
            case "hybrid":
                return CreateHybridDidDocumentCache();
            default:
                _logger.LogWarning("Unknown cache storage type: {StorageType}, falling back to memory", options.StorageType);
                return CreateMemoryDidDocumentCache();
        }
    }

    public IVerificationResultCache CreateVerificationResultCache(CacheOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (!options.Enabled)
        {
            _logger.LogInformation("Verification result caching is disabled");
            return new NoOpVerificationResultCache();
        }

        switch (options.StorageType?.ToLowerInvariant())
        {
            case "redis":
                return CreateRedisVerificationResultCache();
            case "memory":
                return CreateMemoryVerificationResultCache();
            default:
                _logger.LogWarning("Unknown cache storage type: {StorageType}, falling back to memory", options.StorageType);
                return CreateMemoryVerificationResultCache();
        }
    }

    public ICredentialStatusCache CreateCredentialStatusCache(CacheOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (!options.Enabled)
        {
            _logger.LogInformation("Credential status caching is disabled");
            return new NoOpCredentialStatusCache();
        }

        switch (options.StorageType?.ToLowerInvariant())
        {
            case "redis":
                return CreateRedisCredentialStatusCache();
            case "memory":
                return CreateMemoryCredentialStatusCache();
            default:
                _logger.LogWarning("Unknown cache storage type: {StorageType}, falling back to memory", options.StorageType);
                return CreateMemoryCredentialStatusCache();
        }
    }

    private IDidDocumentCache CreateRedisDidDocumentCache()
    {
        try
        {
            var redisCache = _serviceProvider.GetService<ICache>();
            if (redisCache == null)
            {
                _logger.LogError("Redis cache service not available, falling back to memory cache");
                return CreateMemoryDidDocumentCache();
            }

            return new RedisDidDocumentCache(redisCache, _serviceProvider.GetRequiredService<ILogger<RedisDidDocumentCache>>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Redis DID document cache, falling back to memory cache");
            return CreateMemoryDidDocumentCache();
        }
    }

    private IDidDocumentCache CreateMemoryDidDocumentCache()
    {
        var memoryCache = _serviceProvider.GetRequiredService<IMemoryCache>();
        return new MemoryDidDocumentCache(memoryCache, _serviceProvider.GetRequiredService<ILogger<MemoryDidDocumentCache>>());
    }

    private IDidDocumentCache CreateHybridDidDocumentCache()
    {
        var memoryCache = _serviceProvider.GetRequiredService<IMemoryCache>();
        var redisCache = _serviceProvider.GetService<ICache>();
        
        return new HybridDidDocumentCache(
            memoryCache,
            redisCache,
            _serviceProvider.GetRequiredService<ILogger<HybridDidDocumentCache>>(),
            useRedis: redisCache != null,
            useMemory: true);
    }

    private IVerificationResultCache CreateRedisVerificationResultCache()
    {
        try
        {
            var redisCache = _serviceProvider.GetService<ICache>();
            if (redisCache == null)
            {
                _logger.LogError("Redis cache service not available, falling back to memory cache");
                return CreateMemoryVerificationResultCache();
            }

            return new RedisVerificationResultCache(redisCache, _serviceProvider.GetRequiredService<ILogger<RedisVerificationResultCache>>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Redis verification result cache, falling back to memory cache");
            return CreateMemoryVerificationResultCache();
        }
    }

    private IVerificationResultCache CreateMemoryVerificationResultCache()
    {
        var memoryCache = _serviceProvider.GetRequiredService<IMemoryCache>();
        return new MemoryVerificationResultCache(memoryCache, _serviceProvider.GetRequiredService<ILogger<MemoryVerificationResultCache>>());
    }

    private ICredentialStatusCache CreateRedisCredentialStatusCache()
    {
        try
        {
            var redisCache = _serviceProvider.GetService<ICache>();
            if (redisCache == null)
            {
                _logger.LogError("Redis cache service not available, falling back to memory cache");
                return CreateMemoryCredentialStatusCache();
            }

            return new RedisCredentialStatusCache(redisCache, _serviceProvider.GetRequiredService<ILogger<RedisCredentialStatusCache>>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Redis credential status cache, falling back to memory cache");
            return CreateMemoryCredentialStatusCache();
        }
    }

    private ICredentialStatusCache CreateMemoryCredentialStatusCache()
    {
        var memoryCache = _serviceProvider.GetRequiredService<IMemoryCache>();
        return new MemoryCredentialStatusCache(memoryCache, _serviceProvider.GetRequiredService<ILogger<MemoryCredentialStatusCache>>());
    }
}

/// <summary>
/// Cache options for different cache types
/// </summary>
public class CacheOptions
{
    public bool Enabled { get; set; } = true;
    public string StorageType { get; set; } = "Memory";
    public int TtlMinutes { get; set; } = 60;
    public string RedisConnectionString { get; set; } = string.Empty;
    public bool UseHybrid { get; set; } = false;
}

