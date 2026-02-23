using System.Text.Json;
using StackExchange.Redis;
using System.Linq;

namespace Mamey.Security.Redis.Serializers;

/// <summary>
/// Redis serializer for automatically hashing string values.
/// Note: Hashing is one-way, so deserialization returns the stored hash value.
/// </summary>
public class HashedRedisSerializer
{
    private readonly ISecurityProvider _securityProvider;
    private readonly JsonSerializerOptions _jsonOptions;

    public HashedRedisSerializer(ISecurityProvider securityProvider, JsonSerializerOptions? jsonOptions = null)
    {
        _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions();
    }

    /// <summary>
    /// Serializes and hashes a value for Redis storage.
    /// </summary>
    public RedisValue Serialize<T>(T value)
    {
        if (value == null)
            return RedisValue.Null;

        if (value is string stringValue)
        {
            // Handle empty strings specially - don't hash them
            if (stringValue == string.Empty)
                return string.Empty;

            // Only hash if the value doesn't look like it's already hashed
            // (SHA-512 produces 128 character hex strings)
            if (stringValue.Length == 128 && stringValue.All(c => char.IsLetterOrDigit(c)))
            {
                // Already hashed, return as-is
                return stringValue;
            }

            return _securityProvider.Hash(stringValue);
        }

        var json = JsonSerializer.Serialize(value, _jsonOptions);
        return _securityProvider.Hash(json);
    }

    /// <summary>
    /// Deserializes a hashed value from Redis storage.
    /// Note: Hashing is one-way, so this returns the stored hash value.
    /// </summary>
    public T? Deserialize<T>(RedisValue value)
    {
        // Check if the value is null or has no value
        if (value.IsNull || !value.HasValue)
        {
            // For strings, check if it's an empty string (RedisValue can represent empty strings)
            if (typeof(T) == typeof(string) && value == RedisValue.EmptyString)
                return (T)(object)string.Empty;
            return default;
        }

        // Check if the value is an empty string
        if (value == RedisValue.EmptyString || value == string.Empty)
        {
            if (typeof(T) == typeof(string))
                return (T)(object)string.Empty;
            return default;
        }

        var hashedValue = value.ToString();
        
        // Handle empty strings specially - if ToString() returns empty or null, it's an empty string
        if (hashedValue == null || hashedValue == string.Empty)
        {
            if (typeof(T) == typeof(string))
                return (T)(object)string.Empty;
            return default;
        }

        // Hashing is one-way, return stored value as-is
        if (typeof(T) == typeof(string))
            return (T)(object)hashedValue;

        // For non-string types, we can't deserialize from a hash
        throw new InvalidOperationException("Cannot deserialize non-string types from hashed values. Hashing is one-way.");
    }
}

