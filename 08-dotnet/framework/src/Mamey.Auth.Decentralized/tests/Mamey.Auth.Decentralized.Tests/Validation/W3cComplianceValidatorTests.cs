using FluentAssertions;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Validation;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.Validation;

/// <summary>
/// Tests for the W3cComplianceValidator class covering W3C DID 1.1 compliance.
/// </summary>
public class W3cComplianceValidatorTests
{
    #region Happy Path Tests

    [Fact]
    public async Task ValidateAsync_WithW3CCompliantDidDocument_ShouldReturnCompliantResult()
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

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithMinimalW3CCompliantDidDocument_ShouldReturnCompliantResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithSupportedVerificationMethodTypes_ShouldReturnCompliantResult()
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
                },
                new()
                {
                    Id = "did:web:example.com#key-2",
                    Type = "RsaVerificationKey2018",
                    Controller = "did:web:example.com",
                    PublicKeyJwk = new Dictionary<string, object>
                    {
                        ["kty"] = "RSA",
                        ["n"] = "n-value",
                        ["e"] = "AQAB"
                    }
                }
            }
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithValidJwk_ShouldReturnCompliantResult()
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
            }
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithValidMultibase_ShouldReturnCompliantResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            VerificationMethod = new List<VerificationMethod>
            {
                new()
                {
                    Id = "did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw#key-1",
                    Type = "Ed25519VerificationKey2020",
                    Controller = "did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw",
                    PublicKeyMultibase = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
                }
            }
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    #endregion

    #region Unhappy Path Tests

    [Fact]
    public async Task ValidateAsync_WithNullDidDocument_ShouldReturnNonCompliantResult()
    {
        // Arrange
        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(null!);

        // Assert
        result.IsCompliant.Should().BeFalse();
        result.Errors.Should().Contain("DID Document cannot be null for W3C compliance validation.");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingContext_ShouldReturnNonCompliantResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com"
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeFalse();
        result.Errors.Should().Contain("DID Document must include 'https://www.w3.org/ns/did/v1' in its @context.");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidContext_ShouldReturnNonCompliantResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://example.com/context" }
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeFalse();
        result.Errors.Should().Contain("DID Document must include 'https://www.w3.org/ns/did/v1' in its @context.");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingId_ShouldReturnNonCompliantResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeFalse();
        result.Errors.Should().Contain("DID Document must have an 'id' property.");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidId_ShouldReturnNonCompliantResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "invalid-did",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeFalse();
        result.Errors.Should().Contain("DID Document 'id' is not a valid DID according to W3C spec: invalid-did");
    }

    [Fact]
    public async Task ValidateAsync_WithUnsupportedVerificationMethodType_ShouldReturnWarning()
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
                    Type = "UnsupportedVerificationMethodType",
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

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeTrue();
        result.Warnings.Should().Contain("Unsupported verification method type: UnsupportedVerificationMethodType");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingPublicKeyMaterial_ShouldReturnNonCompliantResult()
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

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeFalse();
        result.Errors.Should().Contain("Verification method did:web:example.com#key-1 must have public key material");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidJwk_ShouldReturnNonCompliantResult()
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
                        ["invalid"] = "jwk"
                    }
                }
            }
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeFalse();
        result.Errors.Should().Contain("JWK for did:web:example.com#key-1 missing 'kty' field");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidMultibase_ShouldReturnNonCompliantResult()
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
                    PublicKeyMultibase = "invalid-multibase"
                }
            }
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeFalse();
        result.Errors.Should().Contain("Multibase for did:web:example.com#key-1 does not start with 'z' (base58btc) or is empty.");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingServiceType_ShouldReturnNonCompliantResult()
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

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeFalse();
        result.Errors.Should().Contain("Service did:web:example.com#service-1 must have a 'type'.");
    }

    [Fact]
    public async Task ValidateAsync_WithMissingServiceEndpoint_ShouldReturnNonCompliantResult()
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

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeFalse();
        result.Errors.Should().Contain("Service did:web:example.com#service-1 must have a 'serviceEndpoint'.");
    }

    [Fact]
    public async Task ValidateAsync_WithInvalidServiceEndpoint_ShouldReturnNonCompliantResult()
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

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeFalse();
        result.Errors.Should().Contain("Service did:web:example.com#service-1 'serviceEndpoint' is not a valid URI.");
    }

    #endregion

    #region W3C DID 1.1 Compliance Tests

    [Fact]
    public async Task ValidateAsync_WithW3CCompliantDidDocument_ShouldReturnCompliantResult_V2()
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

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithEd25519VerificationKey2020_ShouldReturnCompliantResult()
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
            }
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithRsaVerificationKey2018_ShouldReturnCompliantResult()
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
                    Type = "RsaVerificationKey2018",
                    Controller = "did:web:example.com",
                    PublicKeyJwk = new Dictionary<string, object>
                    {
                        ["kty"] = "RSA",
                        ["n"] = "n-value",
                        ["e"] = "AQAB"
                    }
                }
            }
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithJsonWebKey2020_ShouldReturnCompliantResult()
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
                    Type = "JsonWebKey2020",
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

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task ValidateAsync_WithEmptyVerificationMethod_ShouldReturnCompliantResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            VerificationMethod = new List<VerificationMethod>()
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithEmptyService_ShouldReturnCompliantResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Service = new List<ServiceEndpoint>()
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithNullVerificationMethod_ShouldReturnCompliantResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            VerificationMethod = null
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_WithNullService_ShouldReturnCompliantResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Service = null
        };

        var validator = new W3cComplianceValidator(Mock.Of<ILogger<W3cComplianceValidator>>());

        // Act
        var result = await validator.ValidateAsync(didDocument);

        // Assert
        result.IsCompliant.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    #endregion
}
