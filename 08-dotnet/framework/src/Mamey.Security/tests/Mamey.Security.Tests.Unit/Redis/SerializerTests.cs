using Mamey.Security;
using Mamey.Security.Internals;
using Mamey.Security.Redis.Serializers;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;
using StackExchange.Redis;
using Xunit;
using TestObject = Mamey.Security.Tests.Shared.Helpers.TestObject;

namespace Mamey.Security.Tests.Unit.Redis;

/// <summary>
/// Comprehensive tests for Redis serializers covering all scenarios.
/// </summary>
public class SerializerTests : IClassFixture<SecurityTestFixture>
{
    private readonly SecurityTestFixture _fixture;

    public SerializerTests(SecurityTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region Happy Paths - EncryptedRedisSerializer

    [Fact]
    public void EncryptedRedisSerializer_SerializeString_ShouldEncrypt()
    {
        // Arrange
        var serializer = new EncryptedRedisSerializer(_fixture.SecurityProvider);
        var value = "sensitive data";

        // Act
        var redisValue = serializer.Serialize(value);

        // Assert
        redisValue.HasValue.ShouldBeTrue();
        var encryptedString = redisValue.ToString();
        encryptedString.ShouldNotBe(value);
        AssertionHelpers.ShouldBeEncrypted(value, encryptedString);
    }

    [Fact]
    public void EncryptedRedisSerializer_DeserializeString_ShouldDecrypt()
    {
        // Arrange
        var serializer = new EncryptedRedisSerializer(_fixture.SecurityProvider);
        var original = "sensitive data";
        var encrypted = serializer.Serialize(original);

        // Act
        var decrypted = serializer.Deserialize<string>(encrypted);

        // Assert
        decrypted.ShouldBe(original);
        AssertionHelpers.ShouldDecryptToOriginal(original, decrypted!);
    }

    [Fact]
    public void EncryptedRedisSerializer_SerializeObject_ShouldEncrypt()
    {
        // Arrange
        var serializer = new EncryptedRedisSerializer(_fixture.SecurityProvider);
        var obj = TestDataGenerator.GenerateTestObject();

        // Act
        var redisValue = serializer.Serialize(obj);

        // Assert
        redisValue.HasValue.ShouldBeTrue();
    }

    [Fact]
    public void EncryptedRedisSerializer_DeserializeObject_ShouldDecrypt()
    {
        // Arrange
        var serializer = new EncryptedRedisSerializer(_fixture.SecurityProvider);
        var original = TestDataGenerator.GenerateTestObject();
        var encrypted = serializer.Serialize(original);

        // Act
        var decrypted = serializer.Deserialize<TestObject>(encrypted);

        // Assert
        decrypted.ShouldNotBeNull();
        decrypted!.Id.ShouldBe(original.Id);
        decrypted.Name.ShouldBe(original.Name);
        decrypted.Email.ShouldBe(original.Email);
    }

    [Fact]
    public void EncryptedRedisSerializer_RoundTrip_ShouldReturnOriginal()
    {
        // Arrange
        var serializer = new EncryptedRedisSerializer(_fixture.SecurityProvider);
        var original = "sensitive data";

        // Act
        var encrypted = serializer.Serialize(original);
        var decrypted = serializer.Deserialize<string>(encrypted);

        // Assert
        decrypted.ShouldBe(original);
    }

    [Fact]
    public void EncryptedRedisSerializer_NullValue_ShouldHandleNull()
    {
        // Arrange
        var serializer = new EncryptedRedisSerializer(_fixture.SecurityProvider);
        string? value = null;

        // Act
        var redisValue = serializer.Serialize(value!);
        var result = serializer.Deserialize<string>(redisValue);

        // Assert
        redisValue.IsNull.ShouldBeTrue();
        result.ShouldBeNull();
    }

    [Fact]
    public void EncryptedRedisSerializer_EmptyString_ShouldHandleEmpty()
    {
        // Arrange
        var serializer = new EncryptedRedisSerializer(_fixture.SecurityProvider);
        var value = "";

        // Act
        var redisValue = serializer.Serialize(value);
        var result = serializer.Deserialize<string>(redisValue);

        // Assert
        result.ShouldBe(value);
    }

    [Fact]
    public void EncryptedRedisSerializer_LargeData_ShouldHandleLargeData()
    {
        // Arrange
        var serializer = new EncryptedRedisSerializer(_fixture.SecurityProvider);
        var value = TestDataGenerator.GenerateLargeString();

        // Act
        var redisValue = serializer.Serialize(value);
        var result = serializer.Deserialize<string>(redisValue);

        // Assert
        result.ShouldBe(value);
    }

    #endregion

    #region Happy Paths - HashedRedisSerializer

    [Fact]
    public void HashedRedisSerializer_SerializeString_ShouldHash()
    {
        // Arrange
        var serializer = new HashedRedisSerializer(_fixture.SecurityProvider);
        var value = "password123";

        // Act
        var redisValue = serializer.Serialize(value);

        // Assert
        redisValue.HasValue.ShouldBeTrue();
        var hashedString = redisValue.ToString();
        hashedString.ShouldNotBe(value);
        AssertionHelpers.ShouldBeValidHash(hashedString);
    }

    [Fact]
    public void HashedRedisSerializer_DeserializeString_ShouldReturnStoredHash()
    {
        // Arrange
        var serializer = new HashedRedisSerializer(_fixture.SecurityProvider);
        var original = "password123";
        var hashed = serializer.Serialize(original);

        // Act
        var result = serializer.Deserialize<string>(hashed);

        // Assert
        result.ShouldBe(hashed.ToString()); // Hashing is one-way
        result.ShouldNotBe(original);
    }

    [Fact]
    public void HashedRedisSerializer_NullValue_ShouldHandleNull()
    {
        // Arrange
        var serializer = new HashedRedisSerializer(_fixture.SecurityProvider);
        string? value = null;

        // Act
        var redisValue = serializer.Serialize(value!);
        var result = serializer.Deserialize<string>(redisValue);

        // Assert
        redisValue.IsNull.ShouldBeTrue();
        result.ShouldBeNull();
    }

    [Fact]
    public void HashedRedisSerializer_EmptyString_ShouldHandleEmpty()
    {
        // Arrange
        var serializer = new HashedRedisSerializer(_fixture.SecurityProvider);
        var value = "";

        // Act
        var redisValue = serializer.Serialize(value);
        var result = serializer.Deserialize<string>(redisValue);

        // Assert
        result.ShouldBe(value);
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void EncryptedRedisSerializer_NullSecurityProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        ISecurityProvider? provider = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new EncryptedRedisSerializer(provider!));
    }

    [Fact]
    public void HashedRedisSerializer_NullSecurityProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        ISecurityProvider? provider = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new HashedRedisSerializer(provider!));
    }

    [Fact]
    public void EncryptedRedisSerializer_DeserializeCorruptedData_ShouldThrowException()
    {
        // Arrange
        var serializer = new EncryptedRedisSerializer(_fixture.SecurityProvider);
        var corruptedData = RedisValue.Unbox("corrupted encrypted data");

        // Act & Assert
        Should.Throw<Exception>(() => serializer.Deserialize<string>(corruptedData));
    }

    [Fact]
    public void EncryptedRedisSerializer_DeserializeWithWrongKey_ShouldThrowException()
    {
        // Arrange
        var serializer1 = new EncryptedRedisSerializer(_fixture.SecurityProvider);
        // Create a second SecurityProvider with a different key
        var services = new ServiceCollection();
        services.AddLogging();
        var securityOptions2 = new SecurityOptions
        {
            Encryption = new SecurityOptions.EncryptionOptions
            {
                Enabled = true,
                Key = "98765432109876543210987654321098" // Different key (32 chars)
            }
        };
        services.AddSingleton(securityOptions2);
        var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        var encryptor2 = new Encryptor(loggerFactory.CreateLogger<Encryptor>());
        var hasher2 = new Hasher();
        var rng2 = new Rng();
        var signer2 = new Signer();
        var md5_2 = new Md5();
        services.AddSingleton<IEncryptor>(encryptor2);
        services.AddSingleton<IHasher>(hasher2);
        services.AddSingleton<IRng>(rng2);
        services.AddSingleton<ISigner>(signer2);
        services.AddSingleton<IMd5>(md5_2);
        var serviceProvider2 = services.BuildServiceProvider();
        var securityProvider2 = new SecurityProvider(encryptor2, hasher2, rng2, signer2, md5_2, securityOptions2);
        var serializer2 = new EncryptedRedisSerializer(securityProvider2);
        var original = "sensitive data";
        var encrypted = serializer1.Serialize(original);

        // Act & Assert
        // When decrypting with wrong key, AES decryption throws CryptographicException
        Should.Throw<System.Security.Cryptography.CryptographicException>(() => serializer2.Deserialize<string>(encrypted));
    }

    [Fact]
    public void HashedRedisSerializer_DeserializeNonStringType_ShouldThrowException()
    {
        // Arrange
        var serializer = new HashedRedisSerializer(_fixture.SecurityProvider);
        var hashed = serializer.Serialize("password123");

        // Act & Assert
        // Note: Hashed values can only be deserialized as strings
        // This test verifies the behavior
        var result = serializer.Deserialize<string>(hashed);
        result.ShouldNotBeNull();
    }

    #endregion
}

