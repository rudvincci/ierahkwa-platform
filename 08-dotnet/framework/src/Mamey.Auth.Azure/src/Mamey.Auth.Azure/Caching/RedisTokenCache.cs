namespace Mamey.Auth.Azure.Caching;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

public class RedisTokenCache : IRedisTokenCache
{
    private readonly IDatabase _redisDatabase;

    public RedisTokenCache(IConnectionMultiplexer redis)
    {
        _redisDatabase = redis.GetDatabase();
    }

    public async Task<string> GetCachedTokenAsync(string key)
    {
        return await _redisDatabase.StringGetAsync(key);
    }

    public async Task SetCachedTokenAsync(string key, string token)
    {
        var expiration = TimeSpan.FromMinutes(60); // Adjust expiration based on your needs
        await _redisDatabase.StringSetAsync(key, token, expiration);
    }
}
