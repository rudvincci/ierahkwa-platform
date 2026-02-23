using FluentAssertions;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Handlers;
using Mamey.Auth.Decentralized.Caching;
using Mamey.Auth.Decentralized.Persistence.Write;
using Mamey.Auth.Decentralized.Persistence.Read;
using Mamey.Auth.Decentralized.VerifiableCredentials;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.Handlers;

/// <summary>
/// Tests for the DecentralizedHandler class covering W3C DID 1.1 compliance.
/// </summary>
public class DecentralizedHandlerTests
{
    #region Happy Path Tests

    [Fact]
    public async Task ResolveDidAsync_WithValidDid_ShouldReturnSuccessfulResult()
    {
        // Arrange
        var did = "did:web:example.com";
        var didDocument = new DidDocument
        {
            Id = did,
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        var mockCache = Mock.Of<IDidDocumentCache>();
        var mockUnitOfWork = Mock.Of<IDidUnitOfWork>();
        var mockReadRepository = Mock.Of<IDidDocumentReadRepository>();
        var mockVerificationMethodRepository = Mock.Of<IVerificationMethodReadRepository>();
        var mockServiceEndpointRepository = Mock.Of<IServiceEndpointReadRepository>();
        var mockVcValidator = Mock.Of<IVcValidator>();
        var mockVcJwtHandler = Mock.Of<IVcJwtHandler>();
        var mockVcJsonLdHandler = Mock.Of<IVcJsonLdHandler>();

        Mock.Get(mockReadRepository)
            .Setup(r => r.GetByIdAsync(did, It.IsAny<CancellationToken>()))
            .ReturnsAsync(didDocument);

        var handler = new DecentralizedHandler(
            mockCache,
            mockUnitOfWork,
            mockReadRepository,
            mockVerificationMethodRepository,
            mockServiceEndpointRepository,
            mockVcValidator,
            mockVcJwtHandler,
            mockVcJsonLdHandler
        );

        // Act
        var result = await handler.ResolveDidAsync(did);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.DidDocument.Should().Be(didDocument);
    }

    [Fact]
    public async Task ResolveDidAsync_WithCachedDid_ShouldReturnCachedResult()
    {
        // Arrange
        var did = "did:web:example.com";
        var didDocument = new DidDocument
        {
            Id = did,
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        var mockCache = Mock.Of<IDidDocumentCache>();
        var mockUnitOfWork = Mock.Of<IDidUnitOfWork>();
        var mockReadRepository = Mock.Of<IDidDocumentReadRepository>();
        var mockVerificationMethodRepository = Mock.Of<IVerificationMethodReadRepository>();
        var mockServiceEndpointRepository = Mock.Of<IServiceEndpointReadRepository>();
        var mockVcValidator = Mock.Of<IVcValidator>();
        var mockVcJwtHandler = Mock.Of<IVcJwtHandler>();
        var mockVcJsonLdHandler = Mock.Of<IVcJsonLdHandler>();

        Mock.Get(mockCache)
            .Setup(c => c.GetAsync(did, It.IsAny<CancellationToken>()))
            .ReturnsAsync(didDocument);

        var handler = new DecentralizedHandler(
            mockCache,
            mockUnitOfWork,
            mockReadRepository,
            mockVerificationMethodRepository,
            mockServiceEndpointRepository,
            mockVcValidator,
            mockVcJwtHandler,
            mockVcJsonLdHandler
        );

        // Act
        var result = await handler.ResolveDidAsync(did);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.DidDocument.Should().Be(didDocument);
    }

    [Fact]
    public async Task CreateDidDocumentAsync_WithValidDidDocument_ShouldReturnTrue()
    {
        // Arrange
        var did = "did:web:example.com";
        var didDocument = new DidDocument
        {
            Id = did,
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        var mockCache = Mock.Of<IDidDocumentCache>();
        var mockUnitOfWork = Mock.Of<IDidUnitOfWork>();
        var mockReadRepository = Mock.Of<IDidDocumentReadRepository>();
        var mockVerificationMethodRepository = Mock.Of<IVerificationMethodReadRepository>();
        var mockServiceEndpointRepository = Mock.Of<IServiceEndpointReadRepository>();
        var mockVcValidator = Mock.Of<IVcValidator>();
        var mockVcJwtHandler = Mock.Of<IVcJwtHandler>();
        var mockVcJsonLdHandler = Mock.Of<IVcJsonLdHandler>();

        Mock.Get(mockUnitOfWork)
            .Setup(u => u.DidDocuments.AddAsync(It.IsAny<DidDocumentEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        Mock.Get(mockUnitOfWork)
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new DecentralizedHandler(
            mockCache,
            mockUnitOfWork,
            mockReadRepository,
            mockVerificationMethodRepository,
            mockServiceEndpointRepository,
            mockVcValidator,
            mockVcJwtHandler,
            mockVcJsonLdHandler
        );

        // Act
        var result = await handler.CreateDidDocumentAsync(did, didDocument);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateDidDocumentAsync_WithValidDidDocument_ShouldReturnTrue()
    {
        // Arrange
        var did = "did:web:example.com";
        var didDocument = new DidDocument
        {
            Id = did,
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        var mockCache = Mock.Of<IDidDocumentCache>();
        var mockUnitOfWork = Mock.Of<IDidUnitOfWork>();
        var mockReadRepository = Mock.Of<IDidDocumentReadRepository>();
        var mockVerificationMethodRepository = Mock.Of<IVerificationMethodReadRepository>();
        var mockServiceEndpointRepository = Mock.Of<IServiceEndpointReadRepository>();
        var mockVcValidator = Mock.Of<IVcValidator>();
        var mockVcJwtHandler = Mock.Of<IVcJwtHandler>();
        var mockVcJsonLdHandler = Mock.Of<IVcJsonLdHandler>();

        Mock.Get(mockUnitOfWork)
            .Setup(u => u.DidDocuments.Update(It.IsAny<DidDocumentEntity>()))
            .Returns(new DidDocumentEntity());

        Mock.Get(mockUnitOfWork)
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new DecentralizedHandler(
            mockCache,
            mockUnitOfWork,
            mockReadRepository,
            mockVerificationMethodRepository,
            mockServiceEndpointRepository,
            mockVcValidator,
            mockVcJwtHandler,
            mockVcJsonLdHandler
        );

        // Act
        var result = await handler.UpdateDidDocumentAsync(did, didDocument);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteDidDocumentAsync_WithValidDid_ShouldReturnTrue()
    {
        // Arrange
        var did = "did:web:example.com";
        var didDocument = new DidDocument
        {
            Id = did,
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        var mockCache = Mock.Of<IDidDocumentCache>();
        var mockUnitOfWork = Mock.Of<IDidUnitOfWork>();
        var mockReadRepository = Mock.Of<IDidDocumentReadRepository>();
        var mockVerificationMethodRepository = Mock.Of<IVerificationMethodReadRepository>();
        var mockServiceEndpointRepository = Mock.Of<IServiceEndpointReadRepository>();
        var mockVcValidator = Mock.Of<IVcValidator>();
        var mockVcJwtHandler = Mock.Of<IVcJwtHandler>();
        var mockVcJsonLdHandler = Mock.Of<IVcJsonLdHandler>();

        Mock.Get(mockUnitOfWork)
            .Setup(u => u.DidDocuments.Remove(It.IsAny<DidDocumentEntity>()))
            .Returns(new DidDocumentEntity());

        Mock.Get(mockUnitOfWork)
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new DecentralizedHandler(
            mockCache,
            mockUnitOfWork,
            mockReadRepository,
            mockVerificationMethodRepository,
            mockServiceEndpointRepository,
            mockVcValidator,
            mockVcJwtHandler,
            mockVcJsonLdHandler
        );

        // Act
        var result = await handler.DeleteDidDocumentAsync(did);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Unhappy Path Tests

    [Fact]
    public async Task ResolveDidAsync_WithInvalidDid_ShouldReturnFailedResult()
    {
        // Arrange
        var did = "invalid-did";

        var mockCache = Mock.Of<IDidDocumentCache>();
        var mockUnitOfWork = Mock.Of<IDidUnitOfWork>();
        var mockReadRepository = Mock.Of<IDidDocumentReadRepository>();
        var mockVerificationMethodRepository = Mock.Of<IVerificationMethodReadRepository>();
        var mockServiceEndpointRepository = Mock.Of<IServiceEndpointReadRepository>();
        var mockVcValidator = Mock.Of<IVcValidator>();
        var mockVcJwtHandler = Mock.Of<IVcJwtHandler>();
        var mockVcJsonLdHandler = Mock.Of<IVcJsonLdHandler>();

        var handler = new DecentralizedHandler(
            mockCache,
            mockUnitOfWork,
            mockReadRepository,
            mockVerificationMethodRepository,
            mockServiceEndpointRepository,
            mockVcValidator,
            mockVcJwtHandler,
            mockVcJsonLdHandler
        );

        // Act
        var result = await handler.ResolveDidAsync(did);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ResolveDidAsync_WithNotFoundDid_ShouldReturnNotFoundResult()
    {
        // Arrange
        var did = "did:web:example.com";

        var mockCache = Mock.Of<IDidDocumentCache>();
        var mockUnitOfWork = Mock.Of<IDidUnitOfWork>();
        var mockReadRepository = Mock.Of<IDidDocumentReadRepository>();
        var mockVerificationMethodRepository = Mock.Of<IVerificationMethodReadRepository>();
        var mockServiceEndpointRepository = Mock.Of<IServiceEndpointReadRepository>();
        var mockVcValidator = Mock.Of<IVcValidator>();
        var mockVcJwtHandler = Mock.Of<IVcJwtHandler>();
        var mockVcJsonLdHandler = Mock.Of<IVcJsonLdHandler>();

        Mock.Get(mockCache)
            .Setup(c => c.GetAsync(did, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DidDocument?)null);

        Mock.Get(mockReadRepository)
            .Setup(r => r.GetByIdAsync(did, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DidDocument?)null);

        var handler = new DecentralizedHandler(
            mockCache,
            mockUnitOfWork,
            mockReadRepository,
            mockVerificationMethodRepository,
            mockServiceEndpointRepository,
            mockVcValidator,
            mockVcJwtHandler,
            mockVcJsonLdHandler
        );

        // Act
        var result = await handler.ResolveDidAsync(did);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateDidDocumentAsync_WithInvalidDid_ShouldReturnFalse()
    {
        // Arrange
        var did = "invalid-did";
        var didDocument = new DidDocument
        {
            Id = did,
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        var mockCache = Mock.Of<IDidDocumentCache>();
        var mockUnitOfWork = Mock.Of<IDidUnitOfWork>();
        var mockReadRepository = Mock.Of<IDidDocumentReadRepository>();
        var mockVerificationMethodRepository = Mock.Of<IVerificationMethodReadRepository>();
        var mockServiceEndpointRepository = Mock.Of<IServiceEndpointReadRepository>();
        var mockVcValidator = Mock.Of<IVcValidator>();
        var mockVcJwtHandler = Mock.Of<IVcJwtHandler>();
        var mockVcJsonLdHandler = Mock.Of<IVcJsonLdHandler>();

        var handler = new DecentralizedHandler(
            mockCache,
            mockUnitOfWork,
            mockReadRepository,
            mockVerificationMethodRepository,
            mockServiceEndpointRepository,
            mockVcValidator,
            mockVcJwtHandler,
            mockVcJsonLdHandler
        );

        // Act
        var result = await handler.CreateDidDocumentAsync(did, didDocument);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateDidDocumentAsync_WithInvalidDid_ShouldReturnFalse()
    {
        // Arrange
        var did = "invalid-did";
        var didDocument = new DidDocument
        {
            Id = did,
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        var mockCache = Mock.Of<IDidDocumentCache>();
        var mockUnitOfWork = Mock.Of<IDidUnitOfWork>();
        var mockReadRepository = Mock.Of<IDidDocumentReadRepository>();
        var mockVerificationMethodRepository = Mock.Of<IVerificationMethodReadRepository>();
        var mockServiceEndpointRepository = Mock.Of<IServiceEndpointReadRepository>();
        var mockVcValidator = Mock.Of<IVcValidator>();
        var mockVcJwtHandler = Mock.Of<IVcJwtHandler>();
        var mockVcJsonLdHandler = Mock.Of<IVcJsonLdHandler>();

        var handler = new DecentralizedHandler(
            mockCache,
            mockUnitOfWork,
            mockReadRepository,
            mockVerificationMethodRepository,
            mockServiceEndpointRepository,
            mockVcValidator,
            mockVcJwtHandler,
            mockVcJsonLdHandler
        );

        // Act
        var result = await handler.UpdateDidDocumentAsync(did, didDocument);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteDidDocumentAsync_WithInvalidDid_ShouldReturnFalse()
    {
        // Arrange
        var did = "invalid-did";

        var mockCache = Mock.Of<IDidDocumentCache>();
        var mockUnitOfWork = Mock.Of<IDidUnitOfWork>();
        var mockReadRepository = Mock.Of<IDidDocumentReadRepository>();
        var mockVerificationMethodRepository = Mock.Of<IVerificationMethodReadRepository>();
        var mockServiceEndpointRepository = Mock.Of<IServiceEndpointReadRepository>();
        var mockVcValidator = Mock.Of<IVcValidator>();
        var mockVcJwtHandler = Mock.Of<IVcJwtHandler>();
        var mockVcJsonLdHandler = Mock.Of<IVcJsonLdHandler>();

        var handler = new DecentralizedHandler(
            mockCache,
            mockUnitOfWork,
            mockReadRepository,
            mockVerificationMethodRepository,
            mockServiceEndpointRepository,
            mockVcValidator,
            mockVcJwtHandler,
            mockVcJsonLdHandler
        );

        // Act
        var result = await handler.DeleteDidDocumentAsync(did);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region W3C DID 1.1 Compliance Tests

    [Fact]
    public async Task ResolveDidAsync_WithW3CCompliantDid_ShouldReturnSuccessfulResult()
    {
        // Arrange - DID compliant with W3C DID 1.1 spec
        var did = "did:web:example.com";
        var didDocument = new DidDocument
        {
            Id = did,
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            VerificationMethod = new List<VerificationMethod>
            {
                new()
                {
                    Id = "did:web:example.com#key-1",
                    Type = "Ed25519VerificationKey2020",
                    Controller = "did:web:example.com",
                    PublicKeyJwk = new Dictionary<string, object>
                    {
                        ["kty"] = "OKP",
                        ["crv"] = "Ed25519",
                        ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
                    }
                }
            },
            Service = new List<ServiceEndpoint>
            {
                new()
                {
                    Id = "did:web:example.com#service-1",
                    Type = "DIDCommMessaging",
                    ServiceEndpointUrl = "https://example.com/messaging"
                }
            }
        };

        var mockCache = Mock.Of<IDidDocumentCache>();
        var mockUnitOfWork = Mock.Of<IDidUnitOfWork>();
        var mockReadRepository = Mock.Of<IDidDocumentReadRepository>();
        var mockVerificationMethodRepository = Mock.Of<IVerificationMethodReadRepository>();
        var mockServiceEndpointRepository = Mock.Of<IServiceEndpointReadRepository>();
        var mockVcValidator = Mock.Of<IVcValidator>();
        var mockVcJwtHandler = Mock.Of<IVcJwtHandler>();
        var mockVcJsonLdHandler = Mock.Of<IVcJsonLdHandler>();

        Mock.Get(mockReadRepository)
            .Setup(r => r.GetByIdAsync(did, It.IsAny<CancellationToken>()))
            .ReturnsAsync(didDocument);

        var handler = new DecentralizedHandler(
            mockCache,
            mockUnitOfWork,
            mockReadRepository,
            mockVerificationMethodRepository,
            mockServiceEndpointRepository,
            mockVcValidator,
            mockVcJwtHandler,
            mockVcJsonLdHandler
        );

        // Act
        var result = await handler.ResolveDidAsync(did);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.DidDocument.Should().Be(didDocument);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task ResolveDidAsync_WithNullDid_ShouldReturnFailedResult()
    {
        // Arrange
        string? did = null;

        var mockCache = Mock.Of<IDidDocumentCache>();
        var mockUnitOfWork = Mock.Of<IDidUnitOfWork>();
        var mockReadRepository = Mock.Of<IDidDocumentReadRepository>();
        var mockVerificationMethodRepository = Mock.Of<IVerificationMethodReadRepository>();
        var mockServiceEndpointRepository = Mock.Of<IServiceEndpointReadRepository>();
        var mockVcValidator = Mock.Of<IVcValidator>();
        var mockVcJwtHandler = Mock.Of<IVcJwtHandler>();
        var mockVcJsonLdHandler = Mock.Of<IVcJsonLdHandler>();

        var handler = new DecentralizedHandler(
            mockCache,
            mockUnitOfWork,
            mockReadRepository,
            mockVerificationMethodRepository,
            mockServiceEndpointRepository,
            mockVcValidator,
            mockVcJwtHandler,
            mockVcJsonLdHandler
        );

        // Act
        var result = await handler.ResolveDidAsync(did!);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ResolveDidAsync_WithEmptyDid_ShouldReturnFailedResult()
    {
        // Arrange
        var did = "";

        var mockCache = Mock.Of<IDidDocumentCache>();
        var mockUnitOfWork = Mock.Of<IDidUnitOfWork>();
        var mockReadRepository = Mock.Of<IDidDocumentReadRepository>();
        var mockVerificationMethodRepository = Mock.Of<IVerificationMethodReadRepository>();
        var mockServiceEndpointRepository = Mock.Of<IServiceEndpointReadRepository>();
        var mockVcValidator = Mock.Of<IVcValidator>();
        var mockVcJwtHandler = Mock.Of<IVcJwtHandler>();
        var mockVcJsonLdHandler = Mock.Of<IVcJsonLdHandler>();

        var handler = new DecentralizedHandler(
            mockCache,
            mockUnitOfWork,
            mockReadRepository,
            mockVerificationMethodRepository,
            mockServiceEndpointRepository,
            mockVcValidator,
            mockVcJwtHandler,
            mockVcJsonLdHandler
        );

        // Act
        var result = await handler.ResolveDidAsync(did);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    #endregion
}
