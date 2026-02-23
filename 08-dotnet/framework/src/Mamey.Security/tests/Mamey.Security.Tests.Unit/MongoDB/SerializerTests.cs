using Mamey.Security;
using Mamey.Security.Internals;
using Mamey.Security.MongoDB.Serializers;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using Shouldly;
using Xunit;

namespace Mamey.Security.Tests.Unit.MongoDB;

/// <summary>
/// Comprehensive tests for MongoDB serializers covering all scenarios.
/// </summary>
public class SerializerTests : IClassFixture<SecurityTestFixture>
{
    private readonly SecurityTestFixture _fixture;

    public SerializerTests(SecurityTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region Happy Paths - EncryptedStringSerializer

    [Fact]
    public void EncryptedStringSerializer_Serialize_ShouldEncrypt()
    {
        // Arrange
        var serializer = new EncryptedStringSerializer(_fixture.SecurityProvider);
        var value = "sensitive data";

        // Act - Serialize within a document field
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        writer.WriteStartDocument();
        writer.WriteName("field");
        var serializationContext = BsonSerializationContext.CreateRoot(writer);
        serializer.Serialize(serializationContext, new BsonSerializationArgs { NominalType = typeof(string) }, value);
        writer.WriteEndDocument();
        writer.Flush();
        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        reader.ReadStartDocument();
        reader.ReadName("field");
        var encryptedValue = reader.ReadString();
        reader.ReadEndDocument();

        // Assert
        encryptedValue.ShouldNotBeNullOrEmpty();
        encryptedValue.ShouldNotBe(value);
        AssertionHelpers.ShouldBeEncrypted(value, encryptedValue);
    }

    [Fact]
    public void EncryptedStringSerializer_Deserialize_ShouldDecrypt()
    {
        // Arrange
        var serializer = new EncryptedStringSerializer(_fixture.SecurityProvider);
        var original = "sensitive data";
        var encrypted = _fixture.SecurityProvider.Encrypt(original);

        // Act - Deserialize from a document field
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        writer.WriteStartDocument();
        writer.WriteName("field");
        writer.WriteString(encrypted);
        writer.WriteEndDocument();
        writer.Flush();
        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        reader.ReadStartDocument();
        reader.ReadName("field");
        var deserializationContext = BsonDeserializationContext.CreateRoot(reader);
        var decrypted = serializer.Deserialize(deserializationContext, new BsonDeserializationArgs { NominalType = typeof(string) });
        reader.ReadEndDocument();

        // Assert
        decrypted.ShouldBe(original);
        AssertionHelpers.ShouldDecryptToOriginal(original, decrypted);
    }

    [Fact]
    public void EncryptedStringSerializer_RoundTrip_ShouldReturnOriginal()
    {
        // Arrange
        var serializer = new EncryptedStringSerializer(_fixture.SecurityProvider);
        var original = "sensitive data";

        // Act - Serialize within a document field
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        writer.WriteStartDocument();
        writer.WriteName("field");
        var serializationContext = BsonSerializationContext.CreateRoot(writer);
        serializer.Serialize(serializationContext, new BsonSerializationArgs { NominalType = typeof(string) }, original);
        writer.WriteEndDocument();
        writer.Flush();
        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        reader.ReadStartDocument();
        reader.ReadName("field");
        var deserializationContext = BsonDeserializationContext.CreateRoot(reader);
        var decrypted = serializer.Deserialize(deserializationContext, new BsonDeserializationArgs { NominalType = typeof(string) });
        reader.ReadEndDocument();

        // Assert
        decrypted.ShouldBe(original);
    }

    [Fact]
    public void EncryptedStringSerializer_NullValue_ShouldHandleNull()
    {
        // Arrange
        var serializer = new EncryptedStringSerializer(_fixture.SecurityProvider);
        string? value = null;

        // Act - Serialize within a document field
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        writer.WriteStartDocument();
        writer.WriteName("field");
        var serializationContext = BsonSerializationContext.CreateRoot(writer);
        serializer.Serialize(serializationContext, new BsonSerializationArgs { NominalType = typeof(string) }, value!);
        writer.WriteEndDocument();
        writer.Flush();
        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        reader.ReadStartDocument();
        reader.ReadName("field");
        var deserializationContext = BsonDeserializationContext.CreateRoot(reader);
        var result = serializer.Deserialize(deserializationContext, new BsonDeserializationArgs { NominalType = typeof(string) });
        reader.ReadEndDocument();

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void EncryptedStringSerializer_EmptyString_ShouldHandleEmpty()
    {
        // Arrange
        var serializer = new EncryptedStringSerializer(_fixture.SecurityProvider);
        var value = "";

        // Act - Serialize within a document field
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        writer.WriteStartDocument();
        writer.WriteName("field");
        var serializationContext = BsonSerializationContext.CreateRoot(writer);
        serializer.Serialize(serializationContext, new BsonSerializationArgs { NominalType = typeof(string) }, value);
        writer.WriteEndDocument();
        writer.Flush();
        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        reader.ReadStartDocument();
        reader.ReadName("field");
        var deserializationContext = BsonDeserializationContext.CreateRoot(reader);
        var result = serializer.Deserialize(deserializationContext, new BsonDeserializationArgs { NominalType = typeof(string) });
        reader.ReadEndDocument();

        // Assert
        result.ShouldBe(value);
    }

    [Fact]
    public void EncryptedStringSerializer_LargeData_ShouldHandleLargeData()
    {
        // Arrange
        var serializer = new EncryptedStringSerializer(_fixture.SecurityProvider);
        var value = TestDataGenerator.GenerateLargeString();

        // Act - Serialize within a document field
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        writer.WriteStartDocument();
        writer.WriteName("field");
        var serializationContext = BsonSerializationContext.CreateRoot(writer);
        serializer.Serialize(serializationContext, new BsonSerializationArgs { NominalType = typeof(string) }, value);
        writer.WriteEndDocument();
        writer.Flush();
        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        reader.ReadStartDocument();
        reader.ReadName("field");
        var deserializationContext = BsonDeserializationContext.CreateRoot(reader);
        var result = serializer.Deserialize(deserializationContext, new BsonDeserializationArgs { NominalType = typeof(string) });
        reader.ReadEndDocument();

        // Assert
        result.ShouldBe(value);
    }

    #endregion

    #region Happy Paths - HashedStringSerializer

    [Fact]
    public void HashedStringSerializer_Serialize_ShouldHash()
    {
        // Arrange
        var serializer = new HashedStringSerializer(_fixture.SecurityProvider);
        var value = "password123";

        // Act - Serialize within a document field
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        writer.WriteStartDocument();
        writer.WriteName("field");
        var serializationContext = BsonSerializationContext.CreateRoot(writer);
        serializer.Serialize(serializationContext, new BsonSerializationArgs { NominalType = typeof(string) }, value);
        writer.WriteEndDocument();
        writer.Flush();
        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        reader.ReadStartDocument();
        reader.ReadName("field");
        var hashedValue = reader.ReadString();
        reader.ReadEndDocument();

        // Assert
        hashedValue.ShouldNotBeNullOrEmpty();
        hashedValue.ShouldNotBe(value);
        AssertionHelpers.ShouldBeValidHash(hashedValue);
    }

    [Fact]
    public void HashedStringSerializer_Deserialize_ShouldReturnStoredHash()
    {
        // Arrange
        var serializer = new HashedStringSerializer(_fixture.SecurityProvider);
        var original = "password123";
        var hashed = _fixture.SecurityProvider.Hash(original);

        // Act - Deserialize from a document field
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        writer.WriteStartDocument();
        writer.WriteName("field");
        writer.WriteString(hashed);
        writer.WriteEndDocument();
        writer.Flush();
        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        reader.ReadStartDocument();
        reader.ReadName("field");
        var deserializationContext = BsonDeserializationContext.CreateRoot(reader);
        var result = serializer.Deserialize(deserializationContext, new BsonDeserializationArgs { NominalType = typeof(string) });
        reader.ReadEndDocument();

        // Assert
        result.ShouldBe(hashed); // Hashing is one-way
        result.ShouldNotBe(original);
    }

    [Fact]
    public void HashedStringSerializer_NullValue_ShouldHandleNull()
    {
        // Arrange
        var serializer = new HashedStringSerializer(_fixture.SecurityProvider);
        string? value = null;

        // Act - Serialize within a document field
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        writer.WriteStartDocument();
        writer.WriteName("field");
        var serializationContext = BsonSerializationContext.CreateRoot(writer);
        serializer.Serialize(serializationContext, new BsonSerializationArgs { NominalType = typeof(string) }, value!);
        writer.WriteEndDocument();
        writer.Flush();
        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        reader.ReadStartDocument();
        reader.ReadName("field");
        var deserializationContext = BsonDeserializationContext.CreateRoot(reader);
        var result = serializer.Deserialize(deserializationContext, new BsonDeserializationArgs { NominalType = typeof(string) });
        reader.ReadEndDocument();

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public void HashedStringSerializer_EmptyString_ShouldHandleEmpty()
    {
        // Arrange
        var serializer = new HashedStringSerializer(_fixture.SecurityProvider);
        var value = "";

        // Act - Serialize within a document field
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        writer.WriteStartDocument();
        writer.WriteName("field");
        var serializationContext = BsonSerializationContext.CreateRoot(writer);
        serializer.Serialize(serializationContext, new BsonSerializationArgs { NominalType = typeof(string) }, value);
        writer.WriteEndDocument();
        writer.Flush();
        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        reader.ReadStartDocument();
        reader.ReadName("field");
        var deserializationContext = BsonDeserializationContext.CreateRoot(reader);
        var result = serializer.Deserialize(deserializationContext, new BsonDeserializationArgs { NominalType = typeof(string) });
        reader.ReadEndDocument();

        // Assert
        result.ShouldBe(value);
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void EncryptedStringSerializer_NullSecurityProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        ISecurityProvider? provider = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new EncryptedStringSerializer(provider!));
    }

    [Fact]
    public void HashedStringSerializer_NullSecurityProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        ISecurityProvider? provider = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new HashedStringSerializer(provider!));
    }

    [Fact]
    public void EncryptedStringSerializer_DeserializeCorruptedData_ShouldThrowException()
    {
        // Arrange
        var serializer = new EncryptedStringSerializer(_fixture.SecurityProvider);
        var corruptedData = "corrupted encrypted data";

        // Act - Deserialize from a document field
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        writer.WriteStartDocument();
        writer.WriteName("field");
        writer.WriteString(corruptedData);
        writer.WriteEndDocument();
        writer.Flush();
        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        reader.ReadStartDocument();
        reader.ReadName("field");
        var deserializationContext = BsonDeserializationContext.CreateRoot(reader);

        // Act & Assert
        Should.Throw<Exception>(() => serializer.Deserialize(deserializationContext, new BsonDeserializationArgs { NominalType = typeof(string) }));
    }

    [Fact]
    public void EncryptedStringSerializer_DeserializeWithWrongKey_ShouldReturnGarbage()
    {
        // Arrange
        var serializer1 = new EncryptedStringSerializer(_fixture.SecurityProvider);
        
        // Create a second SecurityProvider with a different encryption key
        var services2 = new ServiceCollection();
        services2.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        var securityOptions2 = new SecurityOptions
        {
            Encryption = new SecurityOptions.EncryptionOptions
            {
                Enabled = true,
                Key = "98765432109876543210987654321098" // Different key (32 characters)
            }
        };
        services2.AddSingleton(securityOptions2);
        var loggerFactory2 = services2.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        var encryptor2 = new Encryptor(loggerFactory2.CreateLogger<Encryptor>());
        var hasher2 = new Hasher();
        var rng2 = new Rng();
        var signer2 = new Signer();
        var md52 = new Md5();
        services2.AddSingleton<IEncryptor>(encryptor2);
        services2.AddSingleton<IHasher>(hasher2);
        services2.AddSingleton<IRng>(rng2);
        services2.AddSingleton<ISigner>(signer2);
        services2.AddSingleton<IMd5>(md52);
        services2.AddSingleton<ISecurityProvider>(sp =>
        {
            var encryptor = sp.GetRequiredService<IEncryptor>();
            var hasher = sp.GetRequiredService<IHasher>();
            var rng = sp.GetRequiredService<IRng>();
            var signer = sp.GetRequiredService<ISigner>();
            var md5 = sp.GetRequiredService<IMd5>();
            var options = sp.GetRequiredService<SecurityOptions>();
            return new SecurityProvider(encryptor, hasher, rng, signer, md5, options);
        });
        var serviceProvider2 = services2.BuildServiceProvider();
        var securityProvider2 = serviceProvider2.GetRequiredService<ISecurityProvider>();
        var serializer2 = new EncryptedStringSerializer(securityProvider2);
        var original = "sensitive data";

        // Act - Serialize with first provider
        var stream = new MemoryStream();
        var writer = new BsonBinaryWriter(stream);
        writer.WriteStartDocument();
        writer.WriteName("field");
        var serializationContext = BsonSerializationContext.CreateRoot(writer);
        serializer1.Serialize(serializationContext, new BsonSerializationArgs { NominalType = typeof(string) }, original);
        writer.WriteEndDocument();
        writer.Flush();
        stream.Position = 0;
        var reader = new BsonBinaryReader(stream);
        reader.ReadStartDocument();
        reader.ReadName("field");
        var encryptedValue = reader.ReadString();
        reader.ReadEndDocument();

        // Act - Try to deserialize with second provider (different key)
        stream = new MemoryStream();
        writer = new BsonBinaryWriter(stream);
        writer.WriteStartDocument();
        writer.WriteName("field");
        writer.WriteString(encryptedValue);
        writer.WriteEndDocument();
        writer.Flush();
        stream.Position = 0;
        reader = new BsonBinaryReader(stream);
        reader.ReadStartDocument();
        reader.ReadName("field");
        var deserializationContext = BsonDeserializationContext.CreateRoot(reader);
        
        // Assert - Decrypting with wrong key should either throw an exception or return garbage
        try
        {
            var decrypted = serializer2.Deserialize(deserializationContext, new BsonDeserializationArgs { NominalType = typeof(string) });
            reader.ReadEndDocument();
            // If no exception, the decrypted value should not match the original
            decrypted.ShouldNotBe(original);
        }
        catch (Exception)
        {
            // Exception is acceptable when decrypting with wrong key
            reader.ReadEndDocument();
        }
    }

    #endregion
}
