using Mamey.Security;
using Mamey.Security.EntityFramework;
using Mamey.Security.EntityFramework.ValueConverters;
using Mamey.Security.Internals;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;

namespace Mamey.Security.Tests.Unit.EntityFramework;

/// <summary>
/// Comprehensive tests for EF Core value converters covering all scenarios.
/// </summary>
public class ValueConverterTests : IClassFixture<SecurityTestFixture>
{
    private readonly SecurityTestFixture _fixture;

    public ValueConverterTests(SecurityTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region Happy Paths

    [Fact]
    public void EncryptedValueConverter_EncryptsOnSave_ShouldEncrypt()
    {
        // Arrange
        var converter = new EncryptedValueConverter(_fixture.SecurityProvider);
        var original = "sensitive data";

        // Act
        var encrypted = converter.ConvertToProvider(original) as string;

        // Assert
        encrypted.ShouldNotBeNullOrEmpty();
        encrypted.ShouldNotBe(original);
        AssertionHelpers.ShouldBeEncrypted(original, encrypted!);
    }

    [Fact]
    public void EncryptedValueConverter_DecryptsOnRead_ShouldDecrypt()
    {
        // Arrange
        var converter = new EncryptedValueConverter(_fixture.SecurityProvider);
        var original = "sensitive data";
        var encrypted = converter.ConvertToProvider(original);

        // Act
        var decrypted = converter.ConvertFromProvider(encrypted) as string;

        // Assert
        decrypted.ShouldBe(original);
        AssertionHelpers.ShouldDecryptToOriginal(original, decrypted!);
    }

    [Fact]
    public void EncryptedValueConverter_RoundTrip_ShouldReturnOriginal()
    {
        // Arrange
        var converter = new EncryptedValueConverter(_fixture.SecurityProvider);
        var original = "sensitive data";

        // Act
        var encrypted = converter.ConvertToProvider(original);
        var decrypted = converter.ConvertFromProvider(encrypted);

        // Assert
        decrypted.ShouldBe(original);
    }

    [Fact]
    public void EncryptedValueConverter_NullValue_ShouldHandleNull()
    {
        // Arrange
        var converter = new EncryptedValueConverter(_fixture.SecurityProvider);
        string? value = null;

        // Act
        // Note: EF Core's ValueConverter doesn't process null values - EF Core handles them natively
        // When null is passed to ConvertToProvider, it may return null or empty string depending on implementation
        var encrypted = converter.ConvertToProvider(value!);
        var decrypted = converter.ConvertFromProvider(encrypted);

        // Assert
        // EF Core ValueConverter may return null for null input, or empty string
        // Both are acceptable behaviors - the converter handles null by returning empty string
        // but EF Core's expression tree evaluation might return null
        (decrypted == null || decrypted == string.Empty).ShouldBeTrue();
    }

    [Fact]
    public void EncryptedValueConverter_EmptyString_ShouldHandleEmpty()
    {
        // Arrange
        var converter = new EncryptedValueConverter(_fixture.SecurityProvider);
        var value = "";

        // Act
        var encrypted = converter.ConvertToProvider(value);
        var decrypted = converter.ConvertFromProvider(encrypted);

        // Assert
        decrypted.ShouldBe(value);
    }

    [Fact]
    public void HashedValueConverter_HashesOnSave_ShouldHash()
    {
        // Arrange
        var converter = new HashedValueConverter(_fixture.SecurityProvider);
        var original = "password123";

        // Act
        var hashed = converter.ConvertToProvider(original) as string;

        // Assert
        hashed.ShouldNotBeNullOrEmpty();
        hashed.ShouldNotBe(original);
        AssertionHelpers.ShouldBeValidHash(hashed!);
    }

    [Fact]
    public void HashedValueConverter_ReturnsStoredValueOnRead_ShouldReturnHash()
    {
        // Arrange
        var converter = new HashedValueConverter(_fixture.SecurityProvider);
        var original = "password123";
        var hashed = converter.ConvertToProvider(original);

        // Act
        var result = converter.ConvertFromProvider(hashed);

        // Assert
        result.ShouldBe(hashed); // Hashing is one-way
        result.ShouldNotBe(original);
    }

    [Fact]
    public void HashedValueConverter_NullValue_ShouldHandleNull()
    {
        // Arrange
        var converter = new HashedValueConverter(_fixture.SecurityProvider);
        string? value = null;

        // Act
        // Note: EF Core's ValueConverter doesn't process null values - EF Core handles them natively
        // When null is passed to ConvertToProvider, it may return null or empty string depending on implementation
        var hashed = converter.ConvertToProvider(value!);
        var result = converter.ConvertFromProvider(hashed);

        // Assert
        // EF Core ValueConverter may return null for null input, or empty string
        // Both are acceptable behaviors - the converter handles null by returning empty string
        // but EF Core's expression tree evaluation might return null
        (result == null || result == string.Empty).ShouldBeTrue();
    }

    [Fact]
    public void HashedValueConverter_EmptyString_ShouldHandleEmpty()
    {
        // Arrange
        var converter = new HashedValueConverter(_fixture.SecurityProvider);
        var value = "";

        // Act
        var hashed = converter.ConvertToProvider(value);
        var result = converter.ConvertFromProvider(hashed);

        // Assert
        result.ShouldBe(value);
    }

    [Fact]
    public void EncryptedValueConverter_MultipleProperties_ShouldWorkCorrectly()
    {
        // Arrange
        var converter = new EncryptedValueConverter(_fixture.SecurityProvider);
        var value1 = "data1";
        var value2 = "data2";

        // Act
        var encrypted1 = converter.ConvertToProvider(value1);
        var encrypted2 = converter.ConvertToProvider(value2);
        var decrypted1 = converter.ConvertFromProvider(encrypted1);
        var decrypted2 = converter.ConvertFromProvider(encrypted2);

        // Assert
        decrypted1.ShouldBe(value1);
        decrypted2.ShouldBe(value2);
    }

    #endregion

    #region Sad Paths

    [Fact]
    public void EncryptedValueConverter_NullSecurityProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        ISecurityProvider? provider = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new EncryptedValueConverter(provider!));
    }

    [Fact]
    public void HashedValueConverter_NullSecurityProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        ISecurityProvider? provider = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => new HashedValueConverter(provider!));
    }

    [Fact]
    public void EncryptedValueConverter_DecryptWithWrongKey_ShouldThrowException()
    {
        // Arrange
        var converter1 = new EncryptedValueConverter(_fixture.SecurityProvider);
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
        var securityProvider2 = new SecurityProvider(encryptor2, hasher2, rng2, signer2, md5_2, securityOptions2);
        var converter2 = new EncryptedValueConverter(securityProvider2);
        var original = "sensitive data";
        var encrypted = converter1.ConvertToProvider(original);

        // Act & Assert
        // When decrypting with wrong key, AES decryption throws CryptographicException
        Should.Throw<System.Security.Cryptography.CryptographicException>(() => converter2.ConvertFromProvider(encrypted));
    }

    [Fact]
    public void EncryptedValueConverter_CorruptedData_ShouldThrowException()
    {
        // Arrange
        var converter = new EncryptedValueConverter(_fixture.SecurityProvider);
        var corruptedData = "corrupted encrypted data";

        // Act & Assert
        Should.Throw<Exception>(() => converter.ConvertFromProvider(corruptedData));
    }

    #endregion
}

