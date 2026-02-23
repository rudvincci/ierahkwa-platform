using FluentAssertions;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Handlers;
using Mamey.Auth.DecentralizedIdentifiers.Resolution;
using Mamey.Auth.DecentralizedIdentifiers.VC;
using Mamey.Security;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mamey.Auth.DecentralizedIdentifiers.Tests.Handlers;

public class DidHandlerTests
{
    private readonly Mock<IDidResolver> _didResolverMock;
    private readonly Mock<IKeyProvider> _keyProviderMock;
    private readonly Mock<IProofService> _proofServiceMock;
    private readonly Mock<IVerifiableCredentialValidator> _vcValidatorMock;
    private readonly Mock<ISecurityProvider> _securityProviderMock;
    private readonly Mock<ILogger<DidHandler>> _loggerMock;
    private readonly DidAuthOptions _options;
    private readonly DidHandler _didHandler;

    public DidHandlerTests()
    {
        _didResolverMock = new Mock<IDidResolver>();
        _keyProviderMock = new Mock<IKeyProvider>();
        _proofServiceMock = new Mock<IProofService>();
        _vcValidatorMock = new Mock<IVerifiableCredentialValidator>();
        _securityProviderMock = new Mock<ISecurityProvider>();
        _loggerMock = new Mock<ILogger<DidHandler>>();
        _options = new DidAuthOptions();
        
        _didHandler = new DidHandler(
            _didResolverMock.Object,
            _keyProviderMock.Object,
            _proofServiceMock.Object,
            _vcValidatorMock.Object,
            _securityProviderMock.Object,
            _loggerMock.Object,
            _options
        );
    }

    [Fact]
    public async Task CreateDidToken_WithValidDid_ShouldReturnToken()
    {
        // Arrange
        var did = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";
        var claims = new Dictionary<string, string>
        {
            ["name"] = "John Doe",
            ["email"] = "john@example.com"
        };

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

        _didResolverMock.Setup(x => x.ResolveAsync(did, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = didDocument });

        _keyProviderMock.Setup(x => x.GetPrivateKeyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 });

        // Act
        var result = await _didHandler.CreateDidToken(did, claims);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.Id.Should().Be(did);
        result.Claims.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateDidToken_WithNullDid_ShouldThrowException()
    {
        // Arrange
        string did = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _didHandler.CreateDidToken(did));
    }

    [Fact]
    public async Task CreateDidToken_WithInvalidDid_ShouldThrowException()
    {
        // Arrange
        var did = "invalid-did";

        _didResolverMock.Setup(x => x.ResolveAsync(did, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DidResolutionResult)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _didHandler.CreateDidToken(did));
    }

    [Fact]
    public async Task ValidateDidToken_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var token = "valid-did-token";
        var did = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";

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

        _didResolverMock.Setup(x => x.ResolveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = didDocument });

        _proofServiceMock.Setup(x => x.VerifyProofAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _didHandler.ValidateDidToken(token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateDidToken_WithInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        var token = "invalid-did-token";

        // Act
        var result = await _didHandler.ValidateDidToken(token);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CreateVerifiablePresentation_WithValidInputs_ShouldReturnPresentation()
    {
        // Arrange
        var did = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";
        var credentials = new List<VerifiableCredential>
        {
            new VerifiableCredential
            {
                Id = "test-credential-id",
                Context = new List<object> { "https://www.w3.org/2018/credentials/v1" },
                Type = new List<string> { "VerifiableCredential" },
                Issuer = did,
                IssuanceDate = DateTime.UtcNow,
                CredentialSubject = new Dictionary<string, object>
                {
                    ["id"] = did,
                    ["name"] = "John Doe"
                }
            }
        };
        var options = new PresentationOptions
        {
            Challenge = "test-challenge",
            Domain = "example.com",
            ProofPurpose = "authentication"
        };

        var holderDocument = new DidDocument(
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

        _didResolverMock.Setup(x => x.ResolveAsync(did, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = holderDocument });

        _keyProviderMock.Setup(x => x.GetPrivateKeyAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 });

        _proofServiceMock.Setup(x => x.CreateProofAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Proof
            {
                Type = "Ed25519Signature2020",
                ProofPurpose = "authentication",
                Created = DateTimeOffset.UtcNow,
                VerificationMethod = $"{did}#key-1",
                ProofValue = "test-proof-value"
            });

        // Act
        var result = await _didHandler.CreateVerifiablePresentation(did, credentials, options);

        // Assert
        result.Should().NotBeNull();
        result.Type.Should().Contain("VerifiablePresentation");
        result.Holder.Should().Be(did);
        result.VerifiableCredential.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidatePresentation_WithValidPresentation_ShouldReturnValidResult()
    {
        // Arrange
        var vp = new VerifiablePresentation
        {
            Id = "test-vp-id",
            Context = new List<object> { "https://www.w3.org/2018/credentials/v1" },
            Type = new List<string> { "VerifiablePresentation" },
            Holder = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
            VerifiableCredential = new List<VerifiableCredential>
            {
                new VerifiableCredential
                {
                    Id = "test-credential-id",
                    Context = new List<object> { "https://www.w3.org/2018/credentials/v1" },
                    Type = new List<string> { "VerifiableCredential" },
                    Issuer = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
                    IssuanceDate = DateTime.UtcNow,
                    CredentialSubject = new Dictionary<string, object>
                    {
                        ["id"] = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
                        ["name"] = "John Doe"
                    }
                }
            }
        };

        var holderDocument = new DidDocument(
            new[] { "https://www.w3.org/ns/did/v1" },
            vp.Holder,
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

        _didResolverMock.Setup(x => x.ResolveAsync(vp.Holder, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = holderDocument });

        _proofServiceMock.Setup(x => x.VerifyProofAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _didHandler.ValidatePresentation(vp);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidatePresentation_WithInvalidPresentation_ShouldReturnInvalidResult()
    {
        // Arrange
        var vp = new VerifiablePresentation
        {
            Id = "test-vp-id",
            Context = new List<object> { "https://www.w3.org/2018/credentials/v1" },
            Type = new List<string> { "VerifiablePresentation" },
            Holder = "did:invalid",
            VerifiableCredential = new List<VerifiableCredential>()
        };

        _didResolverMock.Setup(x => x.ResolveAsync(vp.Holder, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DidResolutionResult)null);

        // Act
        var result = await _didHandler.ValidatePresentation(vp);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }
}







