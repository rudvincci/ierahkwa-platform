using System.Text.Json;
using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Security.Redis.Serializers;

/// <summary>
/// Redis serializer for automatically encrypting/decrypting string values.
/// </summary>
public class EncryptedRedisSerializer
{
    private readonly ISecurityProvider _securityProvider;
    private readonly JsonSerializerOptions _jsonOptions;

    public EncryptedRedisSerializer(ISecurityProvider securityProvider, JsonSerializerOptions? jsonOptions = null)
    {
        _securityProvider = securityProvider ?? throw new ArgumentNullException(nameof(securityProvider));
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions();
    }

    /// <summary>
    /// Serializes and encrypts a value for Redis storage.
    /// </summary>
    public RedisValue Serialize<T>(T value)
    {
        if (value == null)
            return RedisValue.Null;

        if (value is string stringValue)
        {
            return _securityProvider.Encrypt(stringValue);
        }

        var json = JsonSerializer.Serialize(value, _jsonOptions);
        return _securityProvider.Encrypt(json);
    }

    /// <summary>
    /// Deserializes and decrypts a value from Redis storage.
    /// </summary>
    public T? Deserialize<T>(RedisValue value)
    {
        if (!value.HasValue || value.IsNull)
            return default;

        var encryptedValue = value.ToString();
        if (string.IsNullOrEmpty(encryptedValue))
            return default;

        var decryptedValue = _securityProvider.Decrypt(encryptedValue);

        if (typeof(T) == typeof(string))
            return (T)(object)(decryptedValue ?? string.Empty);

        return JsonSerializer.Deserialize<T>(decryptedValue, _jsonOptions);
    }
}

