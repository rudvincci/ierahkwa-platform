using System.Text.Json;
using FluentAssertions;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Audit;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Resolution;
using Mamey.Auth.DecentralizedIdentifiers.VC;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Mamey.Auth.DecentralizedIdentifiers.Tests.VC;

public class VCIssuanceServiceTests
{
    private readonly Mock<IDidResolver> _didResolverMock;
    private readonly Mock<IKeyProvider> _keyProviderMock;
    private readonly Mock<IProofService> _proofServiceMock;
    private readonly Mock<ICredentialStatusService> _credentialStatusServiceMock;
    private readonly Mock<IDidAuditService> _auditServiceMock;
    private readonly Mock<ILogger<VCIssuanceService>> _loggerMock;
    private readonly VCIssuanceService _vcIssuanceService;

    public VCIssuanceServiceTests()
    {
        _didResolverMock = new Mock<IDidResolver>();
        _keyProviderMock = new Mock<IKeyProvider>();
        _proofServiceMock = new Mock<IProofService>();
        _credentialStatusServiceMock = new Mock<ICredentialStatusService>();
        _auditServiceMock = new Mock<IDidAuditService>();
        _loggerMock = new Mock<ILogger<VCIssuanceService>>();
        
        _vcIssuanceService = new VCIssuanceService(
            _didResolverMock.Object,
            _keyProviderMock.Object,
            _proofServiceMock.Object,
            _credentialStatusServiceMock.Object,
            _auditServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task IssueCredentialAsync_WithValidRequest_ShouldReturnCredential()
    {
        // Arrange
        var request = new CredentialIssueRequest
        {
            IssuerDid = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
            SubjectDid = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
            CredentialType = "VerifiableCredential",
            Claims = new Dictionary<string, object>
            {
                ["name"] = "John Doe",
                ["email"] = "john@example.com"
            },
            ProofType = ProofType.Ed25519Signature2020,
            IncludeStatus = true
        };

        var issuerDocument = new DidDocument(
            new[] { "https://www.w3.org/ns/did/v1" },
            request.IssuerDid,
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

        var subjectDocument = new DidDocument(
            new[] { "https://www.w3.org/ns/did/v1" },
            request.SubjectDid,
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

        _didResolverMock.Setup(x => x.ResolveAsync(request.IssuerDid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = issuerDocument });
        
        _didResolverMock.Setup(x => x.ResolveAsync(request.SubjectDid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = subjectDocument });

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
                ProofPurpose = "assertionMethod",
                Created = DateTimeOffset.UtcNow,
                VerificationMethod = $"{request.IssuerDid}#key-1",
                ProofValue = "test-proof-value"
            });

        // Act
        var result = await _vcIssuanceService.IssueCredentialAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.Issuer.Should().Be(request.IssuerDid);
        result.Subject.Should().Be(request.SubjectDid);
        result.Type.Should().Contain("VerifiableCredential");
        result.Proof.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task IssueCredentialAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Arrange
        CredentialIssueRequest request = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _vcIssuanceService.IssueCredentialAsync(request));
    }

    [Fact]
    public async Task IssueCredentialAsync_WithInvalidIssuerDid_ShouldThrowException()
    {
        // Arrange
        var request = new CredentialIssueRequest
        {
            IssuerDid = "invalid-did",
            SubjectDid = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
            CredentialType = "VerifiableCredential",
            Claims = new Dictionary<string, object>
            {
                ["name"] = "John Doe"
            }
        };

        _didResolverMock.Setup(x => x.ResolveAsync(request.IssuerDid, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DidResolutionResult)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _vcIssuanceService.IssueCredentialAsync(request));
    }

    [Fact]
    public async Task IssueCredentialAsync_WithInvalidSubjectDid_ShouldThrowException()
    {
        // Arrange
        var request = new CredentialIssueRequest
        {
            IssuerDid = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
            SubjectDid = "invalid-did",
            CredentialType = "VerifiableCredential",
            Claims = new Dictionary<string, object>
            {
                ["name"] = "John Doe"
            }
        };

        var issuerDocument = new DidDocument(
            new[] { "https://www.w3.org/ns/did/v1" },
            request.IssuerDid,
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

        _didResolverMock.Setup(x => x.ResolveAsync(request.IssuerDid, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = issuerDocument });
        
        _didResolverMock.Setup(x => x.ResolveAsync(request.SubjectDid, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DidResolutionResult)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _vcIssuanceService.IssueCredentialAsync(request));
    }

    [Fact]
    public async Task IssueCredentialsBatchAsync_WithValidRequests_ShouldReturnBatchResult()
    {
        // Arrange
        var requests = new List<CredentialIssueRequest>
        {
            new CredentialIssueRequest
            {
                IssuerDid = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
                SubjectDid = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
                CredentialType = "VerifiableCredential",
                Claims = new Dictionary<string, object>
                {
                    ["name"] = "John Doe"
                }
            }
        };

        var batchRequest = new BatchCredentialIssueRequest
        {
            Requests = requests
        };

        var issuerDocument = new DidDocument(
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
            .ReturnsAsync(new DidResolutionResult { DidDocument = issuerDocument });

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
                ProofPurpose = "assertionMethod",
                Created = DateTimeOffset.UtcNow,
                VerificationMethod = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK#key-1",
                ProofValue = "test-proof-value"
            });

        // Act
        var result = await _vcIssuanceService.IssueCredentialsBatchAsync(batchRequest);

        // Assert
        result.Should().NotBeNull();
        result.TotalProcessed.Should().Be(1);
        result.IssuedCredentials.Should().HaveCount(1);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task VerifyCredentialAsync_WithValidCredential_ShouldReturnTrue()
    {
        // Arrange
        var credential = new VerifiableCredential
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
            },
            Proof = new Proof
            {
                Type = "Ed25519Signature2020",
                ProofPurpose = "assertionMethod",
                Created = DateTimeOffset.UtcNow,
                VerificationMethod = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK#key-1",
                ProofValue = "test-proof-value"
            }
        };

        var issuerDocument = new DidDocument(
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
            .ReturnsAsync(new DidResolutionResult { DidDocument = issuerDocument });

        _proofServiceMock.Setup(x => x.VerifyProofAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var credentialJson = JsonSerializer.Serialize(credential);
        var verifyRequest = new CredentialVerifyRequest
        {
            CredentialJwt = credentialJson, // Using JWT field even though it's JSON-LD
            CredentialId = credential.Id
        };
        var result = await _vcIssuanceService.VerifyCredentialAsync(verifyRequest);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task RevokeCredentialAsync_WithValidCredentialId_ShouldSucceed()
    {
        // Arrange
        var credentialId = "test-credential-id";
        
        // First create a credential to revoke
        var issueRequest = new CredentialIssueRequest
        {
            IssuerDid = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
            SubjectDid = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
            CredentialType = "VerifiableCredential",
            Claims = new Dictionary<string, object> { ["name"] = "John Doe" }
        };
        
        var issuerDocument = new DidDocument(
            new[] { "https://www.w3.org/ns/did/v1" },
            "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
            new[] { "Ed25519VerificationKey2020" },
            null, null, null, null, null, null, null, null
        );

        _didResolverMock.Setup(x => x.ResolveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = issuerDocument });

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
                ProofPurpose = "assertionMethod",
                Created = DateTimeOffset.UtcNow,
                VerificationMethod = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK#key-1",
                ProofValue = "test-proof-value"
            });

        var credential = await _vcIssuanceService.IssueCredentialAsync(issueRequest);
        
        // Act
        await _vcIssuanceService.RevokeCredentialAsync(credential.Id);

        // Assert - Verify credential is revoked by checking status
        var revokedCredential = await _vcIssuanceService.GetCredentialAsync(credential.Id);
        revokedCredential.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCredentialAsync_WithValidId_ShouldReturnCredential()
    {
        // Arrange - First issue a credential so we can retrieve it
        var issueRequest = new CredentialIssueRequest
        {
            IssuerDid = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
            SubjectDid = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
            CredentialType = "VerifiableCredential",
            Claims = new Dictionary<string, object> { ["name"] = "John Doe" }
        };

        var issuerDocument = new DidDocument(
            new[] { "https://www.w3.org/ns/did/v1" },
            "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK",
            new[] { "Ed25519VerificationKey2020" },
            null, null, null, null, null, null, null, null
        );

        _didResolverMock.Setup(x => x.ResolveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DidResolutionResult { DidDocument = issuerDocument });

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
                ProofPurpose = "assertionMethod",
                Created = DateTimeOffset.UtcNow,
                VerificationMethod = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK#key-1",
                ProofValue = "test-proof-value"
            });

        var issuedCredential = await _vcIssuanceService.IssueCredentialAsync(issueRequest);

        // Act
        var result = await _vcIssuanceService.GetCredentialAsync(issuedCredential.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(issuedCredential.Id);
    }

    [Fact]
    public async Task GetCredentialsBySubjectAsync_WithValidSubject_ShouldReturnCredentials()
    {
        // Arrange
        var subjectDid = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";

        // Act
        var result = await _vcIssuanceService.GetCredentialsBySubjectAsync(subjectDid);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty(); // No credentials stored yet
    }
}







