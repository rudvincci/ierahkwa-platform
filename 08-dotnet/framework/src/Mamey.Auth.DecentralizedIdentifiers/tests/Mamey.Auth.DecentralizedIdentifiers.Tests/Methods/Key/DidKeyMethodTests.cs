using System.Text.Json;
using FluentAssertions;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Crypto;
using Mamey.Auth.DecentralizedIdentifiers.Methods.Key;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Mamey.Auth.DecentralizedIdentifiers.Tests.Methods.Key;

public class DidKeyMethodTests
{
    private readonly Mock<IKeyPairCryptoProvider> _cryptoProviderMock;
    private readonly Mock<ILogger<DidKeyMethod>> _loggerMock;
    private readonly DidKeyMethod _didKeyMethod;

    public DidKeyMethodTests()
    {
        _cryptoProviderMock = new Mock<IKeyPairCryptoProvider>();
        _cryptoProviderMock.Setup(x => x.KeyType).Returns("Ed25519");
        _cryptoProviderMock.Setup(x => x.VerificationMethodType).Returns("Ed25519VerificationKey2020");
        _loggerMock = new Mock<ILogger<DidKeyMethod>>();
        
        var cryptoProviders = new[] { _cryptoProviderMock.Object };
        _didKeyMethod = new DidKeyMethod(cryptoProviders, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithProvidedKeys_ShouldReturnValidDid()
    {
        // Arrange
        var keyType = "Ed25519";
        var privateKey = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };
        var publicKey = new byte[] { 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64 };
        
        _cryptoProviderMock.Setup(x => x.ExportJwk(It.IsAny<byte[]>()))
            .Returns(new Dictionary<string, object> { ["kty"] = "OKP", ["crv"] = "Ed25519" });
        _cryptoProviderMock.Setup(x => x.ExportBase58(It.IsAny<byte[]>()))
            .Returns("base58key");

        // Act
        var result = await _didKeyMethod.CreateAsync(keyType, publicKey, privateKey);

        // Assert
        result.Should().NotBeNull();
        result.Did.Should().StartWith("did:key:");
        result.Document.Should().NotBeNull();
        result.Document.Id.Should().Be(result.Did);
        result.Document.VerificationMethods.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateWithGeneratedKeysAsync_WithEd25519_ShouldReturnValidDid()
    {
        // Arrange
        var keyType = "Ed25519";
        var privateKey = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };
        var publicKey = new byte[] { 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64 };
        
        _cryptoProviderMock.Setup(x => x.GenerateKeyPairAsync(keyType))
            .ReturnsAsync((privateKey, publicKey));
        _cryptoProviderMock.Setup(x => x.ExportJwk(It.IsAny<byte[]>()))
            .Returns(new Dictionary<string, object> { ["kty"] = "OKP", ["crv"] = "Ed25519" });
        _cryptoProviderMock.Setup(x => x.ExportBase58(It.IsAny<byte[]>()))
            .Returns("base58key");

        // Act
        var result = await _didKeyMethod.CreateWithGeneratedKeysAsync(keyType);

        // Assert
        result.Should().NotBeNull();
        result.Did.Should().StartWith("did:key:");
        result.Document.Should().NotBeNull();
        result.Document.Id.Should().Be(result.Did);
        result.Document.VerificationMethods.Should().NotBeEmpty();
        result.KeyPair.PrivateKey.Should().BeEquivalentTo(privateKey);
        result.KeyPair.PublicKey.Should().BeEquivalentTo(publicKey);
    }

    [Fact]
    public async Task ResolveAsync_WithValidDid_ShouldReturnDidDocument()
    {
        // Arrange
        var did = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";
        
        _cryptoProviderMock.Setup(x => x.ExportJwk(It.IsAny<byte[]>()))
            .Returns(new Dictionary<string, object> { ["kty"] = "OKP", ["crv"] = "Ed25519" });
        _cryptoProviderMock.Setup(x => x.ExportBase58(It.IsAny<byte[]>()))
            .Returns("base58key");

        // Act
        var result = await _didKeyMethod.ResolveAsync(did);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(did);
        result.VerificationMethods.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ResolveAsync_WithInvalidDid_ShouldThrowException()
    {
        // Arrange
        var invalidDid = "did:key:invalid";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _didKeyMethod.ResolveAsync(invalidDid));
    }

    [Fact]
    public async Task CreateAsync_WithNullPrivateKey_ShouldSucceed()
    {
        // Arrange
        var keyType = "Ed25519";
        byte[] privateKey = null;
        var publicKey = new byte[] { 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64 };
        
        _cryptoProviderMock.Setup(x => x.ExportJwk(It.IsAny<byte[]>()))
            .Returns(new Dictionary<string, object> { ["kty"] = "OKP", ["crv"] = "Ed25519" });
        _cryptoProviderMock.Setup(x => x.ExportBase58(It.IsAny<byte[]>()))
            .Returns("base58key");

        // Act
        var result = await _didKeyMethod.CreateAsync(keyType, publicKey, privateKey);

        // Assert
        result.Should().NotBeNull();
        result.Did.Should().StartWith("did:key:");
        result.Document.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAsync_WithNullPublicKey_ShouldThrowArgumentNullException()
    {
        // Arrange
        var keyType = "Ed25519";
        var privateKey = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };
        byte[] publicKey = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _didKeyMethod.CreateAsync(keyType, publicKey, privateKey));
    }
}
