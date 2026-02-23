using System.Text.Json;
using StackExchange.Redis;
using Mamey;

namespace Mamey.Persistence.Redis;

public sealed class RedisCache : ICache
{
    private static readonly HashSet<Type> PrimitiveTypes = new()
    {
        typeof(string),
        typeof(char),
        typeof(int),
        typeof(long),
        typeof(Guid),
        typeof(decimal),
        typeof(double),
        typeof(float),
        typeof(short),
        typeof(uint),
        typeof(ulong)
    };

    private readonly IDatabase _database;
  

    public RedisCache(IDatabase database)
    {
        _database = database;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return default;
        }
            
        var value = await _database.StringGetAsync(key);
        if (string.IsNullOrWhiteSpace(value))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>((string)value!, JsonExtensions.CacheSerializerOptions);
        }
        catch (JsonException)
        {
            // Invalid cache entry (likely old format with $ref metadata), delete it and return null
            // This allows the system to fall back to MongoDB/Postgres and re-cache with the new format
            await _database.KeyDeleteAsync(key);
            return default;
        }
        catch (InvalidOperationException)
        {
            // Invalid cache entry (likely deserialization issue with constructor parameters), delete it and return null
            // This allows the system to fall back to MongoDB/Postgres and re-cache with the new format
            await _database.KeyDeleteAsync(key);
            return default;
        }
    }

    public async Task<IReadOnlyList<T>> GetManyAsync<T>(params string[] keys)
    {
        var values = new List<T>();
        if (keys is null || !keys.Any())
        {
            return values;
        }

        var redisKeys = keys.Select(x => (RedisKey) x).ToArray();
        var redisValues = await _database.StringGetAsync(redisKeys);

        foreach (var (redisValue, index) in redisValues.Select((rv, i) => (rv, i)))
        {
            if (!redisValue.HasValue || redisValue.IsNullOrEmpty)
            {
                continue;
            }

            try
            {
                var deserialized = JsonSerializer.Deserialize<T>((string)redisValue!, JsonExtensions.CacheSerializerOptions);
                if (deserialized != null)
                {
                    values.Add(deserialized);
                }
            }
            catch (JsonException)
            {
                // Invalid cache entry (likely old format with $ref metadata), delete it
                // This allows the system to fall back to MongoDB/Postgres and re-cache with the new format
                await _database.KeyDeleteAsync(redisKeys[index]);
            }
            catch (InvalidOperationException)
            {
                // Invalid cache entry (likely deserialization issue with constructor parameters), delete it
                // This allows the system to fall back to MongoDB/Postgres and re-cache with the new format
                await _database.KeyDeleteAsync(redisKeys[index]);
            }
        }

        return values;
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        => _database.StringSetAsync(key, JsonSerializer.Serialize(value, JsonExtensions.CacheSerializerOptions), expiry);

    public Task DeleteAsync<T>(string key)
        => _database.KeyDeleteAsync(key);

    public Task AddToSetAsync<T>(string key, T value)
        => _database.SetAddAsync(key, AsString(value));

    public Task DeleteFromSetAsync<T>(string key, T value)
        => _database.SetRemoveAsync(key, AsString(value));

    public Task<bool> SetContainsAsync<T>(string key, T value)
        => _database.SetContainsAsync(key, AsString(value));

    public Task<bool> KeyExistsAsync(string key)
        => _database.KeyExistsAsync(key);

    private string AsString<T>(T value)
        => PrimitiveTypes.Contains(typeof(T)) ? value.ToString() : JsonSerializer.Serialize(value, JsonExtensions.CacheSerializerOptions);
}