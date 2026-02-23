using FluentAssertions;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Validation;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.Validation;

/// <summary>
/// Tests for the DidDocumentValidator class covering W3C DID 1.1 compliance.
/// </summary>
public class DidDocumentValidatorTests
{
    #region Happy Path Tests

    [Fact]
    public async Task ValidateAsync_WithValidDidDocument_ShouldReturnValidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
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

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithMinimalDidDocument_ShouldReturnValidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithController_ShouldReturnValidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Controller = "did:web:controller.com"
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithAlsoKnownAs_ShouldReturnValidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            AlsoKnownAs = new List<string> { "https://example.com/identity" }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    #endregion

    #region Unhappy Path Tests

    [Fact]
    public async Task ValidateAsync_WithNullDidDocument_ShouldReturnInvalidResult()
    {
        // Arrange
        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(null!);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("DID Document cannot be null.");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingId_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("DID Document must have an 'id' property.");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidId_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "invalid-did",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("DID Document 'id' is not a valid DID: invalid-did");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingContext_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com"
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("DID Document must have an '@context' property.");
    }

    [Fact]
    public async Task ValidateAsync_WithEmptyContext_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string>()
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("DID Document must have an '@context' property.");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidContext_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://example.com/context" }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("DID Document '@context' must contain 'https://www.w3.org/ns/did/v1'.");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidController_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Controller = "invalid-controller"
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("DID Document 'controller' is not a valid DID: invalid-controller");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidAlsoKnownAs_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            AlsoKnownAs = new List<string> { "invalid-uri" }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("'alsoKnownAs' entry is not a valid URI: invalid-uri");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidVerificationMethod_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            VerificationMethod = new List<VerificationMethod>
            {
                new()
                {
                    Id = "invalid-verification-method",
                    Type = "Ed25519VerificationKey2020",
                    Controller = "did:web:example.com",
                    PublicKeyJwk = new Dictionary<string, object>
                    {
                        ["kty"] = "OKP",
                        ["crv"] = "Ed25519",
                        ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
                    }
                }
            }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Verification method 'id' is not a valid DID URL: invalid-verification-method");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingVerificationMethodId_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            VerificationMethod = new List<VerificationMethod>
            {
                new()
                {
                    Type = "Ed25519VerificationKey2020",
                    Controller = "did:web:example.com",
                    PublicKeyJwk = new Dictionary<string, object>
                    {
                        ["kty"] = "OKP",
                        ["crv"] = "Ed25519",
                        ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
                    }
                }
            }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Verification method must have an 'id'.");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingVerificationMethodType_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            VerificationMethod = new List<VerificationMethod>
            {
                new()
                {
                    Id = "did:web:example.com#key-1",
                    Controller = "did:web:example.com",
                    PublicKeyJwk = new Dictionary<string, object>
                    {
                        ["kty"] = "OKP",
                        ["crv"] = "Ed25519",
                        ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
                    }
                }
            }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Verification method did:web:example.com#key-1 must have a 'type'.");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingVerificationMethodController_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            VerificationMethod = new List<VerificationMethod>
            {
                new()
                {
                    Id = "did:web:example.com#key-1",
                    Type = "Ed25519VerificationKey2020",
                    PublicKeyJwk = new Dictionary<string, object>
                    {
                        ["kty"] = "OKP",
                        ["crv"] = "Ed25519",
                        ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
                    }
                }
            }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Verification method did:web:example.com#key-1 must have a 'controller'.");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidVerificationMethodController_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            VerificationMethod = new List<VerificationMethod>
            {
                new()
                {
                    Id = "did:web:example.com#key-1",
                    Type = "Ed25519VerificationKey2020",
                    Controller = "invalid-controller",
                    PublicKeyJwk = new Dictionary<string, object>
                    {
                        ["kty"] = "OKP",
                        ["crv"] = "Ed25519",
                        ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
                    }
                }
            }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Verification method did:web:example.com#key-1 'controller' is not a valid DID: invalid-controller");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingPublicKeyMaterial_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            VerificationMethod = new List<VerificationMethod>
            {
                new()
                {
                    Id = "did:web:example.com#key-1",
                    Type = "Ed25519VerificationKey2020",
                    Controller = "did:web:example.com"
                }
            }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Verification method did:web:example.com#key-1 must have either publicKeyJwk or publicKeyMultibase");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidService_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Service = new List<ServiceEndpoint>
            {
                new()
                {
                    Id = "invalid-service-id",
                    Type = "DIDCommMessaging",
                    ServiceEndpointUrl = "https://example.com/messaging"
                }
            }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Service 'id' is not a valid DID URL: invalid-service-id");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingServiceId_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Service = new List<ServiceEndpoint>
            {
                new()
                {
                    Type = "DIDCommMessaging",
                    ServiceEndpointUrl = "https://example.com/messaging"
                }
            }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Service must have an 'id'.");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingServiceType_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Service = new List<ServiceEndpoint>
            {
                new()
                {
                    Id = "did:web:example.com#service-1",
                    ServiceEndpointUrl = "https://example.com/messaging"
                }
            }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Service did:web:example.com#service-1 must have a 'type'.");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingServiceEndpoint_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Service = new List<ServiceEndpoint>
            {
                new()
                {
                    Id = "did:web:example.com#service-1",
                    Type = "DIDCommMessaging"
                }
            }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Service did:web:example.com#service-1 must have a 'serviceEndpoint'.");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidServiceEndpoint_ShouldReturnInvalidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Service = new List<ServiceEndpoint>
            {
                new()
                {
                    Id = "did:web:example.com#service-1",
                    Type = "DIDCommMessaging",
                    ServiceEndpointUrl = "invalid-url"
                }
            }
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Service did:web:example.com#service-1 'serviceEndpoint' is not a valid URI: invalid-url");
    }

    #endregion

    #region W3C DID 1.1 Compliance Tests

    [Fact]
    public async Task ValidateAsync_WithW3CCompliantDidDocument_ShouldReturnValidResult()
    {
        // Arrange - DID Document compliant with W3C DID 1.1 spec
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
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

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task ValidateAsync_WithEmptyVerificationMethod_ShouldReturnValidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            VerificationMethod = new List<VerificationMethod>()
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithEmptyService_ShouldReturnValidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Service = new List<ServiceEndpoint>()
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithNullVerificationMethod_ShouldReturnValidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            VerificationMethod = null
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithNullService_ShouldReturnValidResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Service = null
        };

        var validator = new DidDocumentValidator(Mock.Of<ILogger<DidDocumentValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    #endregion
}
