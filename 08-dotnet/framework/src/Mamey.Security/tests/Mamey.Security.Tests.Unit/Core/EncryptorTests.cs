using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mamey.Security;
using Mamey.Security.Internals;
using Mamey.Security.Tests.Shared.Helpers;
using Mamey.Security.Tests.Shared.Utilities;
using Shouldly;
using Xunit;

namespace Mamey.Security.Tests.Unit.Core;

/// <summary>
/// Comprehensive tests for Encryptor class covering all algorithms and edge cases.
/// </summary>
public class EncryptorTests
{
    private readonly IEncryptor _encryptor;
    private readonly ILogger<Encryptor> _logger;

    public EncryptorTests()
    {
        _logger = MockHelpers.CreateMockLogger<Encryptor>();
        _encryptor = new Encryptor(_logger);
    }

    #region AES Encryption Tests - Happy Paths

    [Fact]
    public void Encrypt_Aes_String_ShouldEncryptSuccessfully()
    {
        // Arrange
        var data = "Hello, World!";
        var key = TestKeys.ValidAesKey;

        // Act
        var encrypted = _encryptor.Encrypt(data, key, EncryptionAlgorithms.AES);

        // Assert
        encrypted.ShouldNotBeNullOrEmpty();
        encrypted.ShouldNotBe(data);
        AssertionHelpers.ShouldBeEncrypted(data, encrypted);
    }

    [Fact]
    public void Decrypt_Aes_String_ShouldDecryptSuccessfully()
    {
        // Arrange
        var original = "Hello, World!";
        var key = TestKeys.ValidAesKey;
        var encrypted = _encryptor.Encrypt(original, key, EncryptionAlgorithms.AES);

        // Act
        var decrypted = _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.AES);

        // Assert
        decrypted.ShouldBe(original);
        AssertionHelpers.ShouldDecryptToOriginal(original, decrypted);
    }

    [Fact]
    public void Encrypt_Aes_RoundTrip_ShouldReturnOriginal()
    {
        // Arrange
        var original = "Test data for round-trip encryption";
        var key = TestKeys.ValidAesKey;

        // Act
        var encrypted = _encryptor.Encrypt(original, key, EncryptionAlgorithms.AES);
        var decrypted = _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.AES);

        // Assert
        decrypted.ShouldBe(original);
    }

    [Fact]
    public void Encrypt_Aes_ByteArray_ShouldEncryptSuccessfully()
    {
        // Arrange
        var data = TestDataGenerator.GenerateRandomBytes(100);
        var key = TestKeys.ValidAesKeyBytes;
        var iv = TestKeys.ValidAesIv;

        // Act
        var encrypted = _encryptor.Encrypt(data, iv, key, EncryptionAlgorithms.AES);

        // Assert
        encrypted.ShouldNotBeNull();
        encrypted.Length.ShouldBeGreaterThan(0);
        encrypted.ShouldNotBe(data);
    }

    [Fact]
    public void Decrypt_Aes_ByteArray_ShouldDecryptSuccessfully()
    {
        // Arrange
        var original = TestDataGenerator.GenerateRandomBytes(100);
        var key = TestKeys.ValidAesKeyBytes;
        var iv = TestKeys.ValidAesIv;
        var encrypted = _encryptor.Encrypt(original, iv, key, EncryptionAlgorithms.AES);

        // Act
        var decrypted = _encryptor.Decrypt(encrypted, iv, key, EncryptionAlgorithms.AES);

        // Assert
        decrypted.ShouldBeEquivalentTo(original);
    }

    [Fact]
    public void Encrypt_Aes_LargeData_ShouldEncryptSuccessfully()
    {
        // Arrange
        var data = TestDataGenerator.GenerateLargeString();
        var key = TestKeys.ValidAesKey;

        // Act
        var encrypted = _encryptor.Encrypt(data, key, EncryptionAlgorithms.AES);
        var decrypted = _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.AES);

        // Assert
        decrypted.ShouldBe(data);
    }

    [Fact]
    public void Encrypt_Aes_UnicodeString_ShouldEncryptSuccessfully()
    {
        // Arrange
        var data = TestDataGenerator.GenerateUnicodeString();
        var key = TestKeys.ValidAesKey;

        // Act
        var encrypted = _encryptor.Encrypt(data, key, EncryptionAlgorithms.AES);
        var decrypted = _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.AES);

        // Assert
        decrypted.ShouldBe(data);
    }

    [Fact]
    public void Encrypt_Aes_SpecialCharacters_ShouldEncryptSuccessfully()
    {
        // Arrange
        var data = TestDataGenerator.GenerateStringWithSpecialChars();
        var key = TestKeys.ValidAesKey;

        // Act
        var encrypted = _encryptor.Encrypt(data, key, EncryptionAlgorithms.AES);
        var decrypted = _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.AES);

        // Assert
        decrypted.ShouldBe(data);
    }

    [Fact]
    public void Encrypt_Aes_EmptyString_ShouldEncryptSuccessfully()
    {
        // Arrange
        var data = "";
        var key = TestKeys.ValidAesKey;

        // Act
        var encrypted = _encryptor.Encrypt(data, key, EncryptionAlgorithms.AES);
        var decrypted = _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.AES);

        // Assert
        decrypted.ShouldBe(data);
    }

    [Fact]
    public void Encrypt_Aes_MultipleCycles_ShouldWorkCorrectly()
    {
        // Arrange
        var original = "Test data";
        var key = TestKeys.ValidAesKey;

        // Act
        var encrypted1 = _encryptor.Encrypt(original, key, EncryptionAlgorithms.AES);
        var decrypted1 = _encryptor.Decrypt(encrypted1, key, EncryptionAlgorithms.AES);
        var encrypted2 = _encryptor.Encrypt(decrypted1, key, EncryptionAlgorithms.AES);
        var decrypted2 = _encryptor.Decrypt(encrypted2, key, EncryptionAlgorithms.AES);

        // Assert
        decrypted2.ShouldBe(original);
    }

    #endregion

    #region AES Encryption Tests - Sad Paths

    [Fact]
    public void Encrypt_Aes_NullData_ShouldThrowArgumentException()
    {
        // Arrange
        string? data = null;
        var key = TestKeys.ValidAesKey;

        // Act & Assert
        Should.Throw<ArgumentException>(() => _encryptor.Encrypt(data!, key, EncryptionAlgorithms.AES));
    }

    [Fact]
    public void Encrypt_Aes_NullKey_ShouldThrowArgumentException()
    {
        // Arrange
        var data = "Test data";
        string? key = null;

        // Act & Assert
        Should.Throw<ArgumentException>(() => _encryptor.Encrypt(data, key!, EncryptionAlgorithms.AES));
    }

    [Fact]
    public void Encrypt_Aes_EmptyKey_ShouldThrowArgumentException()
    {
        // Arrange
        var data = "Test data";
        var key = TestKeys.EmptyKey;

        // Act & Assert
        Should.Throw<ArgumentException>(() => _encryptor.Encrypt(data, key, EncryptionAlgorithms.AES));
    }

    [Fact]
    public void Encrypt_Aes_InvalidKeyLength_ShouldThrowArgumentException()
    {
        // Arrange
        var data = "Test data";
        var key = TestKeys.InvalidKey;

        // Act & Assert
        Should.Throw<ArgumentException>(() => _encryptor.Encrypt(data, key, EncryptionAlgorithms.AES));
    }

    [Fact]
    public void Decrypt_Aes_WrongKey_ShouldThrowException()
    {
        // Arrange
        var original = "Test data";
        var key1 = TestKeys.ValidAesKey;
        var key2 = "09876543210987654321098765432109"; // Different valid key
        var encrypted = _encryptor.Encrypt(original, key1, EncryptionAlgorithms.AES);

        // Act & Assert
        Should.Throw<Exception>(() => _encryptor.Decrypt(encrypted, key2, EncryptionAlgorithms.AES));
    }

    [Fact]
    public void Decrypt_Aes_CorruptedData_ShouldThrowException()
    {
        // Arrange
        var key = TestKeys.ValidAesKey;
        var corruptedData = "This is not valid encrypted data!";

        // Act & Assert
        Should.Throw<Exception>(() => _encryptor.Decrypt(corruptedData, key, EncryptionAlgorithms.AES));
    }

    [Fact]
    public void Encrypt_Aes_ByteArray_NullData_ShouldThrowArgumentException()
    {
        // Arrange
        byte[]? data = null;
        var key = TestKeys.ValidAesKeyBytes;
        var iv = TestKeys.ValidAesIv;

        // Act & Assert
        Should.Throw<ArgumentException>(() => _encryptor.Encrypt(data!, iv, key, EncryptionAlgorithms.AES));
    }

    [Fact]
    public void Encrypt_Aes_ByteArray_EmptyData_ShouldThrowArgumentException()
    {
        // Arrange
        var data = Array.Empty<byte>();
        var key = TestKeys.ValidAesKeyBytes;
        var iv = TestKeys.ValidAesIv;

        // Act & Assert
        Should.Throw<ArgumentException>(() => _encryptor.Encrypt(data, iv, key, EncryptionAlgorithms.AES));
    }

    [Fact]
    public void Encrypt_Aes_ByteArray_InvalidIvLength_ShouldThrowException()
    {
        // Arrange
        var data = TestDataGenerator.GenerateRandomBytes(100);
        var key = TestKeys.ValidAesKeyBytes;
        var iv = TestKeys.InvalidIv;

        // Act & Assert
        Should.Throw<Exception>(() => _encryptor.Encrypt(data, iv, key, EncryptionAlgorithms.AES));
    }

    #endregion

    #region TripleDES Encryption Tests - Happy Paths

    [Fact]
    public void Encrypt_TripleDes_String_ShouldEncryptSuccessfully()
    {
        // Arrange
        var data = "Hello, World!";
        var key = TestKeys.ValidTripleDesKey;

        // Act
        var encrypted = _encryptor.Encrypt(data, key, EncryptionAlgorithms.TripleDes);

        // Assert
        encrypted.ShouldNotBeNullOrEmpty();
        encrypted.ShouldNotBe(data);
        AssertionHelpers.ShouldBeEncrypted(data, encrypted);
    }

    [Fact]
    public void Decrypt_TripleDes_String_ShouldDecryptSuccessfully()
    {
        // Arrange
        var original = "Hello, World!";
        var key = TestKeys.ValidTripleDesKey;
        var encrypted = _encryptor.Encrypt(original, key, EncryptionAlgorithms.TripleDes);

        // Act
        var decrypted = _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.TripleDes);

        // Assert
        decrypted.ShouldBe(original);
    }

    [Fact]
    public void Encrypt_TripleDes_RoundTrip_ShouldReturnOriginal()
    {
        // Arrange
        var original = "Test data for round-trip encryption";
        var key = TestKeys.ValidTripleDesKey;

        // Act
        var encrypted = _encryptor.Encrypt(original, key, EncryptionAlgorithms.TripleDes);
        var decrypted = _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.TripleDes);

        // Assert
        decrypted.ShouldBe(original);
    }

    [Fact]
    public void Encrypt_TripleDes_ByteArray_ShouldEncryptSuccessfully()
    {
        // Arrange
        var data = TestDataGenerator.GenerateRandomBytes(100);
        var key = TestKeys.ValidTripleDesKeyBytes;
        var iv = TestKeys.ValidTripleDesIv;

        // Act
        var encrypted = _encryptor.Encrypt(data, iv, key, EncryptionAlgorithms.TripleDes);

        // Assert
        encrypted.ShouldNotBeNull();
        encrypted.Length.ShouldBeGreaterThan(0);
        encrypted.ShouldNotBe(data);
    }

    [Fact]
    public void Decrypt_TripleDes_ByteArray_ShouldDecryptSuccessfully()
    {
        // Arrange
        var original = TestDataGenerator.GenerateRandomBytes(100);
        var key = TestKeys.ValidTripleDesKeyBytes;
        var iv = TestKeys.ValidTripleDesIv;
        var encrypted = _encryptor.Encrypt(original, iv, key, EncryptionAlgorithms.TripleDes);

        // Act
        var decrypted = _encryptor.Decrypt(encrypted, iv, key, EncryptionAlgorithms.TripleDes);

        // Assert
        decrypted.ShouldBeEquivalentTo(original);
    }

    [Fact]
    public void Encrypt_TripleDes_LargeData_ShouldEncryptSuccessfully()
    {
        // Arrange
        var data = TestDataGenerator.GenerateLargeString();
        var key = TestKeys.ValidTripleDesKey;

        // Act
        var encrypted = _encryptor.Encrypt(data, key, EncryptionAlgorithms.TripleDes);
        var decrypted = _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.TripleDes);

        // Assert
        decrypted.ShouldBe(data);
    }

    [Fact]
    public void Encrypt_TripleDes_UnicodeString_ShouldEncryptSuccessfully()
    {
        // Arrange
        var data = TestDataGenerator.GenerateUnicodeString();
        var key = TestKeys.ValidTripleDesKey;

        // Act
        var encrypted = _encryptor.Encrypt(data, key, EncryptionAlgorithms.TripleDes);
        var decrypted = _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.TripleDes);

        // Assert
        decrypted.ShouldBe(data);
    }

    #endregion

    #region TripleDES Encryption Tests - Sad Paths

    [Fact]
    public void Encrypt_TripleDes_InvalidKeyLength_ShouldThrowArgumentException()
    {
        // Arrange
        var data = "Test data";
        var key = TestKeys.ValidAesKey; // Wrong length

        // Act & Assert
        Should.Throw<ArgumentException>(() => _encryptor.Encrypt(data, key, EncryptionAlgorithms.TripleDes));
    }

    [Fact]
    public void Decrypt_TripleDes_WrongKey_ShouldThrowException()
    {
        // Arrange
        var original = "Test data";
        var key1 = TestKeys.ValidTripleDesKey;
        var key2 = "098765432109876543210987"; // Different valid key
        var encrypted = _encryptor.Encrypt(original, key1, EncryptionAlgorithms.TripleDes);

        // Act & Assert
        Should.Throw<Exception>(() => _encryptor.Decrypt(encrypted, key2, EncryptionAlgorithms.TripleDes));
    }

    #endregion

    #region RSA Encryption Tests - Happy Paths

    [Fact]
    public void Encrypt_Rsa_String_ShouldEncryptSuccessfully()
    {
        // Arrange
        var data = "Hello, World!";
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        var key = rsa.ToXmlString(true);

        // Act
        var encrypted = _encryptor.Encrypt(data, key, EncryptionAlgorithms.RSA);

        // Assert
        encrypted.ShouldNotBeNullOrEmpty();
        encrypted.ShouldNotBe(data);
    }

    [Fact]
    public void Decrypt_Rsa_String_ShouldDecryptSuccessfully()
    {
        // Arrange
        var original = "Hello, World!";
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        var key = rsa.ToXmlString(true);
        var encrypted = _encryptor.Encrypt(original, key, EncryptionAlgorithms.RSA);

        // Act
        var decrypted = _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.RSA);

        // Assert
        decrypted.ShouldBe(original);
    }

    [Fact]
    public void Encrypt_Rsa_RoundTrip_ShouldReturnOriginal()
    {
        // Arrange
        var original = "Test data for RSA encryption";
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        var key = rsa.ToXmlString(true);

        // Act
        var encrypted = _encryptor.Encrypt(original, key, EncryptionAlgorithms.RSA);
        var decrypted = _encryptor.Decrypt(encrypted, key, EncryptionAlgorithms.RSA);

        // Assert
        decrypted.ShouldBe(original);
    }

    [Fact]
    public void Encrypt_Rsa_ByteArray_ShouldEncryptSuccessfully()
    {
        // Arrange
        var data = TestDataGenerator.GenerateRandomBytes(100);
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        var publicKey = rsa.ExportRSAPublicKey();
        var privateKey = rsa.ExportRSAPrivateKey();
        var iv = TestKeys.ValidAesIv; // Not used for RSA but required by signature

        // Act
        var encrypted = _encryptor.Encrypt(data, iv, publicKey, EncryptionAlgorithms.RSA);

        // Assert
        encrypted.ShouldNotBeNull();
        encrypted.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Decrypt_Rsa_ByteArray_ShouldDecryptSuccessfully()
    {
        // Arrange
        var original = TestDataGenerator.GenerateRandomBytes(100);
        using var rsa = System.Security.Cryptography.RSA.Create(2048);
        var publicKey = rsa.ExportRSAPublicKey();
        var privateKey = rsa.ExportRSAPrivateKey();
        var iv = TestKeys.ValidAesIv;
        var encrypted = _encryptor.Encrypt(original, iv, publicKey, EncryptionAlgorithms.RSA);

        // Act
        var decrypted = _encryptor.Decrypt(encrypted, iv, privateKey, EncryptionAlgorithms.RSA);

        // Assert
        decrypted.ShouldBeEquivalentTo(original);
    }

    #endregion

    #region RSA Encryption Tests - Sad Paths

    [Fact]
    public void Encrypt_Rsa_InvalidKey_ShouldThrowException()
    {
        // Arrange
        var data = "Test data";
        var key = "Invalid RSA key XML";

        // Act & Assert
        Should.Throw<Exception>(() => _encryptor.Encrypt(data, key, EncryptionAlgorithms.RSA));
    }

    [Fact]
    public void Decrypt_Rsa_WrongKey_ShouldThrowException()
    {
        // Arrange
        var original = "Test data";
        using var rsa1 = System.Security.Cryptography.RSA.Create(2048);
        using var rsa2 = System.Security.Cryptography.RSA.Create(2048);
        var key1 = rsa1.ToXmlString(true);
        var key2 = rsa2.ToXmlString(true);
        var encrypted = _encryptor.Encrypt(original, key1, EncryptionAlgorithms.RSA);

        // Act & Assert
        Should.Throw<Exception>(() => _encryptor.Decrypt(encrypted, key2, EncryptionAlgorithms.RSA));
    }

    #endregion

    #region Invalid Algorithm Tests

    [Fact]
    public void Encrypt_InvalidAlgorithm_ShouldThrowNotSupportedException()
    {
        // Arrange
        var data = "Test data";
        var key = TestKeys.ValidAesKey;

        // Act & Assert
        Should.Throw<NotSupportedException>(() => _encryptor.Encrypt(data, key, (EncryptionAlgorithms)999));
    }

    [Fact]
    public void Decrypt_InvalidAlgorithm_ShouldThrowNotSupportedException()
    {
        // Arrange
        var data = "Test data";
        var key = TestKeys.ValidAesKey;

        // Act & Assert
        Should.Throw<NotSupportedException>(() => _encryptor.Decrypt(data, key, (EncryptionAlgorithms)999));
    }

    #endregion
}

