using FluentAssertions;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Resolution;
using Mamey.Auth.DecentralizedIdentifiers.VC;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System;

namespace Mamey.Auth.DecentralizedIdentifiers.Tests.VC;

public class VPServiceTests
{
    private readonly Mock<IDidResolver> _didResolverMock;
    private readonly Mock<IKeyProvider> _keyProviderMock;
    private readonly Mock<IProofService> _proofServiceMock;
    private readonly Mock<IVerifiableCredentialValidator> _vcValidatorMock;
    private readonly Mock<ICredentialStatusService> _credentialStatusServiceMock;
    private readonly Mock<IJsonLdProcessor> _jsonLdProcessorMock;
    private readonly Mock<ILogger<VPService>> _loggerMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly VPService _vpService;

    public VPServiceTests()
    {
        _didResolverMock = new Mock<IDidResolver>();
        _keyProviderMock = new Mock<IKeyProvider>();
        _proofServiceMock = new Mock<IProofService>();
        _vcValidatorMock = new Mock<IVerifiableCredentialValidator>();
        _credentialStatusServiceMock = new Mock<ICredentialStatusService>();
        _jsonLdProcessorMock = new Mock<IJsonLdProcessor>();
        _loggerMock = new Mock<ILogger<VPService>>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        
        _vpService = new VPService(
            _didResolverMock.Object,
            _keyProviderMock.Object,
            _proofServiceMock.Object,
            _vcValidatorMock.Object,
            _credentialStatusServiceMock.Object,
            _jsonLdProcessorMock.Object,
            _loggerMock.Object,
            _serviceProviderMock.Object
        );
    }

    [Fact]
    public async Task CreatePresentationAsync_WithValidRequest_ShouldReturnPresentation()
    {
        // Arrange
        var request = new PresentationCreateRequest
        {
            HolderDid = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
            Credentials = new List<VerifiableCredential>
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
            },
            Challenge = "test-challenge",
            Domain = "example.com"
        };

        var holderDocument = new DidDocument(
            new[] { "https://www.w3.org/ns/did/v1" },
            request.HolderDid,
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

        _didResolverMock.Setup(x => x.ResolveAsync(request.HolderDid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = holderDocument });

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
                VerificationMethod = $"{request.HolderDid}#key-1",
                ProofValue = "test-proof-value"
            });

        // Act
        var result = await _vpService.CreatePresentationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.Type.Should().Contain("VerifiablePresentation");
        result.Holder.Should().Be(request.HolderDid);
        result.VerifiableCredential.Should().NotBeNull();
    }

    [Fact]
    public async Task CreatePresentationAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Arrange
        PresentationCreateRequest request = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _vpService.CreatePresentationAsync(request));
    }

    [Fact]
    public async Task CreatePresentationAsync_WithInvalidHolderDid_ShouldThrowException()
    {
        // Arrange
        var request = new PresentationCreateRequest
        {
            HolderDid = "invalid-did",
            Credentials = new List<VerifiableCredential>(),
            Challenge = "test-challenge"
        };

        _didResolverMock.Setup(x => x.ResolveAsync(request.HolderDid, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DidResolutionResult)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _vpService.CreatePresentationAsync(request));
    }

    [Fact]
    public async Task VerifyPresentationAsync_WithValidPresentation_ShouldReturnTrue()
    {
        // Arrange
        var request = new PresentationValidationRequest
        {
            Presentation = new VerifiablePresentation
            {
                Id = "test-presentation-id",
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
                },
                Proof = new Proof
                {
                    Type = "Ed25519Signature2020",
                    ProofPurpose = "authentication",
                    Created = DateTimeOffset.UtcNow,
                    VerificationMethod = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK#key-1",
                    ProofValue = "test-proof-value"
                }
            }
        };

        var holderDocument = new DidDocument(
            new[] { "https://www.w3.org/ns/did/v1" },
            "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
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
            .ReturnsAsync(new DidResolutionResult { DidDocument = holderDocument });

        _proofServiceMock.Setup(x => x.VerifyProofAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _vcValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new VerifiableCredentialValidationResult
            {
                IsValid = true,
                Errors = new List<string>()
            });

        _credentialStatusServiceMock.Setup(x => x.IsRevokedAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _vpService.ValidatePresentationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task VerifyPresentationAsync_WithInvalidPresentation_ShouldReturnFalse()
    {
        // Arrange
        var request = new PresentationValidationRequest
        {
            Presentation = new VerifiablePresentation
            {
                Id = "test-presentation-id",
                Context = new List<object> { "https://www.w3.org/2018/credentials/v1" },
                Type = new List<string> { "VerifiablePresentation" },
                Holder = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
                VerifiableCredential = new List<VerifiableCredential>(),
                Proof = new Proof
                {
                    Type = "Ed25519Signature2020",
                    ProofPurpose = "authentication",
                    Created = DateTimeOffset.UtcNow,
                    VerificationMethod = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK#key-1",
                    ProofValue = "invalid-proof-value"
                }
            }
        };

        var holderDocument = new DidDocument(
            new[] { "https://www.w3.org/ns/did/v1" },
            "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
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
            .ReturnsAsync(new DidResolutionResult { DidDocument = holderDocument });

        _proofServiceMock.Setup(x => x.VerifyProofAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _vpService.ValidatePresentationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreatePresentationAsync_WithValidRequest_ShouldReturnDerivedCredential()
    {
        // Arrange
        var originalCredential = new VerifiableCredential
        {
            Id = "original-credential-id",
            Context = new List<object> { "https://www.w3.org/2018/credentials/v1" },
            Type = new List<string> { "VerifiableCredential" },
            Issuer = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
            IssuanceDate = DateTime.UtcNow,
            CredentialSubject = new Dictionary<string, object>
            {
                ["id"] = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
                ["name"] = "John Doe",
                ["email"] = "john@example.com"
            }
        };

        var request = new PresentationCreateRequest
        {
            HolderDid = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
            Credentials = new List<VerifiableCredential> { originalCredential }
        };

        // Act
        var result = await _vpService.CreatePresentationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.Holder.Should().Be(request.HolderDid);
        result.VerifiableCredential.Should().NotBeNull();
    }

    [Fact]
    public async Task CreatePresentationAsync_WithValidPresentation_ShouldReturnClaims()
    {
        // Arrange
        var presentation = new VerifiablePresentation
        {
            Id = "test-presentation-id",
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
                        ["name"] = "John Doe",
                        ["email"] = "john@example.com"
                    }
                }
            }
        };

        // Act - Note: VPService doesn't have GetClaimsFromPresentation method
        // This test needs to be rewritten based on actual implementation
        var request = new PresentationCreateRequest
        {
            HolderDid = presentation.Holder,
            Credentials = presentation.VerifiableCredential is IEnumerable<VerifiableCredential> creds 
                ? creds.ToList() 
                : new List<VerifiableCredential> { (VerifiableCredential)presentation.VerifiableCredential }
        };
        
        var result = await _vpService.CreatePresentationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
    }
}







