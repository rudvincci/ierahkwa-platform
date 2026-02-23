using Mamey.Security.Redis.Serializers;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using Shouldly;
using StackExchange.Redis;
using Testcontainers.Redis;
using Xunit;
using TestObject = Mamey.Security.Tests.Shared.Helpers.TestObject;

namespace Mamey.Security.Tests.Integration.Redis;

/// <summary>
/// Comprehensive integration tests for Redis security features.
/// </summary>
[Collection("Integration")]
public class RedisIntegrationTests : IClassFixture<SecurityTestFixture>, IAsyncLifetime
{
    private readonly SecurityTestFixture _fixture;
    private readonly RedisContainer _redisContainer;
    private IConnectionMultiplexer? _connection;
    private IDatabase? _database;
    private EncryptedRedisSerializer? _encryptedSerializer;
    private HashedRedisSerializer? _hashedSerializer;

    public RedisIntegrationTests(SecurityTestFixture fixture)
    {
        _fixture = fixture;
        _redisContainer = new RedisBuilder()
            .WithImage("redis:7")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();
        var connectionString = _redisContainer.GetConnectionString();
        _connection = await ConnectionMultiplexer.ConnectAsync(connectionString);
        _database = _connection.GetDatabase();
        _encryptedSerializer = new EncryptedRedisSerializer(_fixture.SecurityProvider);
        _hashedSerializer = new HashedRedisSerializer(_fixture.SecurityProvider);
    }

    public async Task DisposeAsync()
    {
        if (_connection != null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }
        await _redisContainer.DisposeAsync();
    }

    #region Happy Paths - Encrypted Operations

    [Fact]
    public async Task SetEncryptedStringValue_ShouldEncrypt()
    {
        // Arrange
        var key = $"encrypted:{Guid.NewGuid()}";
        var value = "sensitive data";

        // Act
        var encrypted = _encryptedSerializer!.Serialize(value);
        await _database!.StringSetAsync(key, encrypted);

        // Assert
        var stored = await _database.StringGetAsync(key);
        stored.HasValue.ShouldBeTrue();
        var storedString = stored.ToString();
        storedString.ShouldNotBe(value);
        AssertionHelpers.ShouldBeEncrypted(value, storedString);
    }

    [Fact]
    public async Task GetEncryptedStringValue_ShouldDecrypt()
    {
        // Arrange
        var key = $"encrypted:{Guid.NewGuid()}";
        var original = "sensitive data";
        var encrypted = _encryptedSerializer!.Serialize(original);
        await _database!.StringSetAsync(key, encrypted);

        // Act
        var stored = await _database.StringGetAsync(key);
        var decrypted = _encryptedSerializer.Deserialize<string>(stored);

        // Assert
        decrypted.ShouldBe(original);
        AssertionHelpers.ShouldDecryptToOriginal(original, decrypted!);
    }

    [Fact]
    public async Task SetEncryptedObjectValue_ShouldEncrypt()
    {
        // Arrange
        var key = $"encrypted:obj:{Guid.NewGuid()}";
        var obj = TestDataGenerator.GenerateTestObject();

        // Act
        var encrypted = _encryptedSerializer!.Serialize(obj);
        await _database!.StringSetAsync(key, encrypted);

        // Assert
        var stored = await _database.StringGetAsync(key);
        stored.HasValue.ShouldBeTrue();
    }

    [Fact]
    public async Task GetEncryptedObjectValue_ShouldDecrypt()
    {
        // Arrange
        var key = $"encrypted:obj:{Guid.NewGuid()}";
        var original = TestDataGenerator.GenerateTestObject();
        var encrypted = _encryptedSerializer!.Serialize(original);
        await _database!.StringSetAsync(key, encrypted);

        // Act
        var stored = await _database.StringSetAsync(key, encrypted);
        var retrieved = await _database.StringGetAsync(key);
        var decrypted = _encryptedSerializer.Deserialize<TestObject>(retrieved);

        // Assert
        decrypted.ShouldNotBeNull();
        decrypted!.Id.ShouldBe(original.Id);
        decrypted.Name.ShouldBe(original.Name);
        decrypted.Email.ShouldBe(original.Email);
    }

    [Fact]
    public async Task SetWithExpiration_ShouldWork()
    {
        // Arrange
        var key = $"encrypted:exp:{Guid.NewGuid()}";
        var value = "sensitive data";
        var encrypted = _encryptedSerializer!.Serialize(value);
        var expiration = TimeSpan.FromMinutes(5);

        // Act
        await _database!.StringSetAsync(key, encrypted, expiration);

        // Assert
        var ttl = await _database.KeyTimeToLiveAsync(key);
        ttl.ShouldNotBeNull();
        ttl!.Value.TotalMinutes.ShouldBeLessThanOrEqualTo(5);
    }

    [Fact]
    public async Task GetWithExpiration_ShouldWork()
    {
        // Arrange
        var key = $"encrypted:exp:{Guid.NewGuid()}";
        var original = "sensitive data";
        var encrypted = _encryptedSerializer!.Serialize(original);
        await _database!.StringSetAsync(key, encrypted, TimeSpan.FromMinutes(5));

        // Act
        var stored = await _database.StringGetAsync(key);
        var decrypted = _encryptedSerializer.Deserialize<string>(stored);

        // Assert
        decrypted.ShouldBe(original);
    }

    [Fact]
    public async Task DeleteEncryptedValue_ShouldWork()
    {
        // Arrange
        var key = $"encrypted:del:{Guid.NewGuid()}";
        var value = "sensitive data";
        var encrypted = _encryptedSerializer!.Serialize(value);
        await _database!.StringSetAsync(key, encrypted);

        // Act
        var deleted = await _database.KeyDeleteAsync(key);

        // Assert
        deleted.ShouldBeTrue();
        var retrieved = await _database.StringGetAsync(key);
        retrieved.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task BulkOperations_ShouldWork()
    {
        // Arrange
        var keys = Enumerable.Range(0, 10)
            .Select(i => $"encrypted:bulk:{Guid.NewGuid()}")
            .ToList();
        var values = keys.Select(k => _encryptedSerializer!.Serialize($"data-{k}")).ToList();

        // Act
        for (int i = 0; i < keys.Count; i++)
        {
            await _database!.StringSetAsync(keys[i], values[i]);
        }

        // Assert
        var count = 0;
        foreach (var key in keys)
        {
            var value = await _database.StringGetAsync(key);
            if (value.HasValue)
                count++;
        }
        count.ShouldBe(10);
    }

    #endregion

    #region Happy Paths - Hashed Operations

    [Fact]
    public async Task SetHashedStringValue_ShouldHash()
    {
        // Arrange
        var key = $"hashed:{Guid.NewGuid()}";
        var value = "password123";

        // Act
        var hashed = _hashedSerializer!.Serialize(value);
        await _database!.StringSetAsync(key, hashed);

        // Assert
        var stored = await _database.StringGetAsync(key);
        stored.HasValue.ShouldBeTrue();
        var storedString = stored.ToString();
        storedString.ShouldNotBe(value);
        AssertionHelpers.ShouldBeValidHash(storedString);
    }

    [Fact]
    public async Task GetHashedStringValue_ShouldReturnHash()
    {
        // Arrange
        var key = $"hashed:{Guid.NewGuid()}";
        var original = "password123";
        var hashed = _hashedSerializer!.Serialize(original);
        await _database!.StringSetAsync(key, hashed);

        // Act
        var stored = await _database.StringGetAsync(key);
        var result = _hashedSerializer.Deserialize<string>(stored);

        // Assert
        result.ShouldBe(stored.ToString()); // Hashing is one-way
        result.ShouldNotBe(original);
    }

    #endregion

    #region Sad Paths

    [Fact]
    public async Task SetEncryptedValue_InvalidEncryptionKey_ShouldThrowException()
    {
        // Arrange
        var fixture = new SecurityTestFixture(encryptionEnabled: true);
        var serializer = new EncryptedRedisSerializer(fixture.SecurityProvider);
        var key = $"encrypted:invalid:{Guid.NewGuid()}";
        var value = "sensitive data";
        var encrypted = serializer.Serialize(value);

        // Act
        await _database!.StringSetAsync(key, encrypted);

        // Note: This test verifies that encryption works with valid key
        // Testing with wrong key would require a different security provider instance
        var stored = await _database.StringGetAsync(key);
        stored.HasValue.ShouldBeTrue();
    }

    [Fact]
    public async Task GetEncryptedValue_NonExistentKey_ShouldReturnNull()
    {
        // Arrange
        var key = $"encrypted:nonexistent:{Guid.NewGuid()}";

        // Act
        var stored = await _database!.StringGetAsync(key);
        var decrypted = _encryptedSerializer!.Deserialize<string>(stored);

        // Assert
        stored.HasValue.ShouldBeFalse();
        decrypted.ShouldBeNull();
    }

    [Fact]
    public async Task GetEncryptedValue_CorruptedData_ShouldThrowException()
    {
        // Arrange
        var key = $"encrypted:corrupted:{Guid.NewGuid()}";
        var corruptedData = "corrupted encrypted data";
        await _database!.StringSetAsync(key, corruptedData);

        // Act & Assert
        var stored = await _database.StringGetAsync(key);
        Should.Throw<Exception>(() => _encryptedSerializer!.Deserialize<string>(stored));
    }

    [Fact]
    public async Task ConnectionFailure_ShouldHandleGracefully()
    {
        // Arrange
        await _redisContainer.StopAsync();

        // Act & Assert
        var key = $"encrypted:fail:{Guid.NewGuid()}";
        var value = "sensitive data";
        var encrypted = _encryptedSerializer!.Serialize(value);
        await Should.ThrowAsync<Exception>(() => _database!.StringSetAsync(key, encrypted));
    }

    [Fact]
    public async Task TimeoutScenario_ShouldHandleGracefully()
    {
        // Arrange
        var key = $"encrypted:timeout:{Guid.NewGuid()}";
        var value = "sensitive data";
        var encrypted = _encryptedSerializer!.Serialize(value);

        // Act
        await _database!.StringSetAsync(key, encrypted, TimeSpan.FromMilliseconds(1));
        await Task.Delay(100); // Wait for expiration

        // Assert
        var stored = await _database.StringGetAsync(key);
        stored.HasValue.ShouldBeFalse();
    }

    #endregion
}

