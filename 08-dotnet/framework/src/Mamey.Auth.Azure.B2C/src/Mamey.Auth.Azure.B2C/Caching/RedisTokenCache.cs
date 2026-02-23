using StackExchange.Redis;

namespace Mamey.Auth.Azure.B2C.Caching;

/// <summary>
/// Implementation of the <see cref="IRedisTokenCache"/> interface for caching tokens using Redis.
/// </summary>
public class RedisTokenCache : IRedisTokenCache
{
    private readonly IDatabase _redisDatabase;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisTokenCache"/> class.
    /// </summary>
    /// <param name="redis">The Redis connection multiplexer.</param>
    public RedisTokenCache(IConnectionMultiplexer redis)
    {
        _redisDatabase = redis.GetDatabase();
    }

    /// <inheritdoc />
    public async Task<string> GetCachedTokenAsync(string key)
    {
        return await _redisDatabase.StringGetAsync(key);
    }

    /// <inheritdoc />
    public async Task SetCachedTokenAsync(string key, string token)
    {
        var expiration = TimeSpan.FromMinutes(60); // Adjust expiration based on your needs
        await _redisDatabase.StringSetAsync(key, token, expiration);
    }
}

