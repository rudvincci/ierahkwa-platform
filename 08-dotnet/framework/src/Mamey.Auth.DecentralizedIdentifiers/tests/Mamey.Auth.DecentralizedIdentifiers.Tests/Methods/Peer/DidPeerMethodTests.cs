using FluentAssertions;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Methods.Peer;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Mamey.Auth.DecentralizedIdentifiers.Tests.Methods.Peer;

public class DidPeerMethodTests
{
    private readonly Mock<IKeyProvider> _keyProviderMock;
    private readonly Mock<ILogger<DidPeerMethod>> _loggerMock;
    private readonly DidPeerMethod _didPeerMethod;

    public DidPeerMethodTests()
    {
        _keyProviderMock = new Mock<IKeyProvider>();
        _loggerMock = new Mock<ILogger<DidPeerMethod>>();
        _didPeerMethod = new DidPeerMethod(_loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithNumalgo0_ShouldReturnValidDid()
    {
        // Arrange
        var options = new PeerMethodOptions
        {
            Numalgo = 0,
            IncludeAuthentication = true,
            IncludeAssertion = true,
            IncludeKeyAgreement = true,
            IncludeCapabilityDelegation = true,
            IncludeCapabilityInvocation = true
        };

        // Note: DidPeerMethod generates keys internally via PeerKeyGen if not provided in options
        // This test doesn't need key provider setup

        // Act
        var result = await _didPeerMethod.CreateAsync(options);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().StartWith("did:peer:");
    }

    [Fact]
    public async Task CreateAsync_WithNumalgo1_ShouldReturnValidDid()
    {
        // Arrange
        var options = new PeerMethodOptions
        {
            Numalgo = 1,
            IncludeAuthentication = true,
            IncludeAssertion = true
        };

        // Note: DidPeerMethod generates keys internally via PeerKeyGen if not provided in options
        // This test doesn't need key provider setup

        // Act
        var result = await _didPeerMethod.CreateAsync(options);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().StartWith("did:peer:");
    }

    [Fact]
    public async Task CreateAsync_WithNumalgo2_ShouldReturnValidDid()
    {
        // Arrange
        var options = new PeerMethodOptions
        {
            Numalgo = 2,
            IncludeAuthentication = true,
            IncludeAssertion = true,
            Services = new List<PeerService>
            {
                new PeerService
                {
                    Id = "service1",
                    Type = "DIDCommMessaging",
                    ServiceEndpoint = "https://example.com/messaging"
                }
            }
        };

        // Note: DidPeerMethod generates keys internally via PeerKeyGen if not provided in options
        // This test doesn't need key provider setup

        // Act
        var result = await _didPeerMethod.CreateAsync(options);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().StartWith("did:peer:");
    }

    [Fact]
    public async Task ResolveAsync_WithValidDid_ShouldReturnDidDocument()
    {
        // Arrange
        var did = "did:peer:0z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";

        // Act
        var result = await _didPeerMethod.ResolveAsync(did);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(did);
    }

    [Fact]
    public async Task ResolveAsync_WithInvalidDid_ShouldThrowException()
    {
        // Arrange
        var invalidDid = "did:peer:invalid";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _didPeerMethod.ResolveAsync(invalidDid));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotSupportedException()
    {
        // Arrange
        var did = "did:peer:0z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";
        var didDocument = new DidDocument(
            new[] { "https://www.w3.org/ns/did/v1" },
            did,
            new[] { "Ed25519VerificationKey2020" },
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        );

        // Act & Assert
        await Assert.ThrowsAsync<NotSupportedException>(() => _didPeerMethod.UpdateAsync(did, didDocument));
    }

    [Fact]
    public async Task DeactivateAsync_ShouldThrowNotSupportedException()
    {
        // Arrange
        var did = "did:peer:0z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";

        // Act & Assert
        await Assert.ThrowsAsync<NotSupportedException>(() => _didPeerMethod.DeactivateAsync(did));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task CreateAsync_WithDifferentNumalgos_ShouldReturnValidDid(int numalgo)
    {
        // Arrange
        var options = new PeerMethodOptions
        {
            Numalgo = numalgo,
            IncludeAuthentication = true,
            IncludeAssertion = true
        };

        // Note: DidPeerMethod generates keys internally via PeerKeyGen if not provided in options
        // This test doesn't need key provider setup

        // Act
        var result = await _didPeerMethod.CreateAsync(options);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().StartWith("did:peer:");
    }

    [Fact]
    public async Task CreateAsync_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Arrange
        PeerMethodOptions options = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _didPeerMethod.CreateAsync(options));
    }

    [Fact]
    public async Task CreateAsync_WithInvalidNumalgo_ShouldThrowArgumentException()
    {
        // Arrange
        var options = new PeerMethodOptions
        {
            Numalgo = 99, // Invalid numalgo
            IncludeAuthentication = true
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _didPeerMethod.CreateAsync(options));
    }
}







