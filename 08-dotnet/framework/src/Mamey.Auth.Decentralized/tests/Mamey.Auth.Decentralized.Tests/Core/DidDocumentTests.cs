using FluentAssertions;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Exceptions;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.Core;

/// <summary>
/// Tests for the DidDocument class covering W3C DID 1.1 compliance.
/// </summary>
public class DidDocumentTests
{
    #region Happy Path Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateDidDocument()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };
        var verificationMethod = new VerificationMethod
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
        };
        var serviceEndpoint = new ServiceEndpoint
        {
            Id = "did:web:example.com#service-1",
            Type = "DIDCommMessaging",
            ServiceEndpointUrl = "https://example.com/messaging"
        };

        // Act
        var didDocument = new DidDocument
        {
            Id = id,
            Context = context,
            VerificationMethod = new List<VerificationMethod> { verificationMethod },
            Service = new List<ServiceEndpoint> { serviceEndpoint }
        };

        // Assert
        didDocument.Id.Should().Be(id);
        didDocument.Context.Should().BeEquivalentTo(context);
        didDocument.VerificationMethod.Should().Contain(verificationMethod);
        didDocument.Service.Should().Contain(serviceEndpoint);
    }

    [Fact]
    public void Constructor_WithMinimalRequiredFields_ShouldCreateDidDocument()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };

        // Act
        var didDocument = new DidDocument
        {
            Id = id,
            Context = context
        };

        // Assert
        didDocument.Id.Should().Be(id);
        didDocument.Context.Should().BeEquivalentTo(context);
    }

    [Fact]
    public void Constructor_WithController_ShouldCreateDidDocument()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };
        var controller = "did:web:controller.com";

        // Act
        var didDocument = new DidDocument
        {
            Id = id,
            Context = context,
            Controller = controller
        };

        // Assert
        didDocument.Controller.Should().Be(controller);
    }

    [Fact]
    public void Constructor_WithAlsoKnownAs_ShouldCreateDidDocument()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };
        var alsoKnownAs = new List<string> { "https://example.com/identity" };

        // Act
        var didDocument = new DidDocument
        {
            Id = id,
            Context = context,
            AlsoKnownAs = alsoKnownAs
        };

        // Assert
        didDocument.AlsoKnownAs.Should().BeEquivalentTo(alsoKnownAs);
    }

    [Fact]
    public void Constructor_WithAuthentication_ShouldCreateDidDocument()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };
        var authentication = new List<string> { "did:web:example.com#key-1" };

        // Act
        var didDocument = new DidDocument
        {
            Id = id,
            Context = context,
            Authentication = authentication
        };

        // Assert
        didDocument.Authentication.Should().BeEquivalentTo(authentication);
    }

    [Fact]
    public void Constructor_WithAssertionMethod_ShouldCreateDidDocument()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };
        var assertionMethod = new List<string> { "did:web:example.com#key-1" };

        // Act
        var didDocument = new DidDocument
        {
            Id = id,
            Context = context,
            AssertionMethod = assertionMethod
        };

        // Assert
        didDocument.AssertionMethod.Should().BeEquivalentTo(assertionMethod);
    }

    [Fact]
    public void Constructor_WithKeyAgreement_ShouldCreateDidDocument()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };
        var keyAgreement = new List<string> { "did:web:example.com#key-1" };

        // Act
        var didDocument = new DidDocument
        {
            Id = id,
            Context = context,
            KeyAgreement = keyAgreement
        };

        // Assert
        didDocument.KeyAgreement.Should().BeEquivalentTo(keyAgreement);
    }

    [Fact]
    public void Constructor_WithCapabilityInvocation_ShouldCreateDidDocument()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };
        var capabilityInvocation = new List<string> { "did:web:example.com#key-1" };

        // Act
        var didDocument = new DidDocument
        {
            Id = id,
            Context = context,
            CapabilityInvocation = capabilityInvocation
        };

        // Assert
        didDocument.CapabilityInvocation.Should().BeEquivalentTo(capabilityInvocation);
    }

    [Fact]
    public void Constructor_WithCapabilityDelegation_ShouldCreateDidDocument()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };
        var capabilityDelegation = new List<string> { "did:web:example.com#key-1" };

        // Act
        var didDocument = new DidDocument
        {
            Id = id,
            Context = context,
            CapabilityDelegation = capabilityDelegation
        };

        // Assert
        didDocument.CapabilityDelegation.Should().BeEquivalentTo(capabilityDelegation);
    }

    #endregion

    #region Unhappy Path Tests

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-id")]
    [InlineData("not-a-did")]
    public void Constructor_WithInvalidId_ShouldThrowInvalidDidException(string invalidId)
    {
        // Act & Assert
        var action = () => new DidDocument
        {
            Id = invalidId,
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        action.Should().Throw<InvalidDidException>();
    }

    [Fact]
    public void Constructor_WithNullContext_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new DidDocument
        {
            Id = "did:web:example.com",
            Context = null!
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithEmptyContext_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new DidDocument
        {
            Id = "did:web:example.com",
            Context = new string[0].ToList()
        };

        action.Should().Throw<ArgumentException>();
    }

    // Removed old test method - replaced with separate null and empty array tests above

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-controller")]
    [InlineData("not-a-did")]
    public void Constructor_WithInvalidController_ShouldThrowInvalidDidException(string invalidController)
    {
        // Act & Assert
        var action = () => new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Controller = invalidController
        };

        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-uri")]
    [InlineData("not-a-uri")]
    public void Constructor_WithInvalidAlsoKnownAs_ShouldThrowArgumentException(string invalidUri)
    {
        // Act & Assert
        var action = () => new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            AlsoKnownAs = new List<string> { invalidUri }
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-reference")]
    [InlineData("not-a-did-url")]
    public void Constructor_WithInvalidAuthentication_ShouldThrowInvalidDidException(string invalidReference)
    {
        // Act & Assert
        var action = () => new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            Authentication = new List<string> { invalidReference }
        };

        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-reference")]
    [InlineData("not-a-did-url")]
    public void Constructor_WithInvalidAssertionMethod_ShouldThrowInvalidDidException(string invalidReference)
    {
        // Act & Assert
        var action = () => new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            AssertionMethod = new List<string> { invalidReference }
        };

        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-reference")]
    [InlineData("not-a-did-url")]
    public void Constructor_WithInvalidKeyAgreement_ShouldThrowInvalidDidException(string invalidReference)
    {
        // Act & Assert
        var action = () => new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            KeyAgreement = new List<string> { invalidReference }
        };

        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-reference")]
    [InlineData("not-a-did-url")]
    public void Constructor_WithInvalidCapabilityInvocation_ShouldThrowInvalidDidException(string invalidReference)
    {
        // Act & Assert
        var action = () => new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            CapabilityInvocation = new List<string> { invalidReference }
        };

        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-reference")]
    [InlineData("not-a-did-url")]
    public void Constructor_WithInvalidCapabilityDelegation_ShouldThrowInvalidDidException(string invalidReference)
    {
        // Act & Assert
        var action = () => new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            CapabilityDelegation = new List<string> { invalidReference }
        };

        action.Should().Throw<InvalidDidException>();
    }

    #endregion

    #region W3C DID 1.1 Compliance Tests

    [Fact]
    public void Constructor_WithW3CCompliantDidDocument_ShouldCreateDidDocument()
    {
        // Arrange - DID Document compliant with W3C DID 1.1 spec
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };
        var verificationMethod = new VerificationMethod
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
        };
        var serviceEndpoint = new ServiceEndpoint
        {
            Id = "did:web:example.com#service-1",
            Type = "DIDCommMessaging",
            ServiceEndpointUrl = "https://example.com/messaging"
        };

        // Act
        var didDocument = new DidDocument
        {
            Id = id,
            Context = context,
            VerificationMethod = new List<VerificationMethod> { verificationMethod },
            Service = new List<ServiceEndpoint> { serviceEndpoint }
        };

        // Assert
        didDocument.Id.Should().Be(id);
        didDocument.Context.Should().Contain("https://www.w3.org/ns/did/v1");
        didDocument.VerificationMethod.Should().Contain(verificationMethod);
        didDocument.Service.Should().Contain(serviceEndpoint);
    }

    [Fact]
    public void Constructor_WithMultipleContexts_ShouldCreateDidDocument()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> 
        { 
            "https://www.w3.org/ns/did/v1",
            "https://w3id.org/security/suites/ed25519-2020/v1"
        };

        // Act
        var didDocument = new DidDocument
        {
            Id = id,
            Context = context
        };

        // Assert
        didDocument.Context.Should().Contain("https://www.w3.org/ns/did/v1");
        didDocument.Context.Should().Contain("https://w3id.org/security/suites/ed25519-2020/v1");
    }

    [Fact]
    public void Constructor_WithMultipleVerificationMethods_ShouldCreateDidDocument()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };
        var verificationMethods = new List<VerificationMethod>
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
        };

        // Act
        var didDocument = new DidDocument
        {
            Id = id,
            Context = context,
            VerificationMethod = verificationMethods
        };

        // Assert
        didDocument.VerificationMethod.Should().HaveCount(2);
        didDocument.VerificationMethod.Should().Contain(vm => vm.Id == "did:web:example.com#key-1");
        didDocument.VerificationMethod.Should().Contain(vm => vm.Id == "did:web:example.com#key-2");
    }

    [Fact]
    public void Constructor_WithMultipleServiceEndpoints_ShouldCreateDidDocument()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };
        var serviceEndpoints = new List<ServiceEndpoint>
        {
            new()
            {
                Id = "did:web:example.com#service-1",
                Type = "DIDCommMessaging",
                ServiceEndpointUrl = "https://example.com/messaging"
            },
            new()
            {
                Id = "did:web:example.com#service-2",
                Type = "LinkedDomains",
                ServiceEndpointUrl = "https://example.com"
            }
        };

        // Act
        var didDocument = new DidDocument
        {
            Id = id,
            Context = context,
            Service = serviceEndpoints
        };

        // Assert
        didDocument.Service.Should().HaveCount(2);
        didDocument.Service.Should().Contain(se => se.Id == "did:web:example.com#service-1");
        didDocument.Service.Should().Contain(se => se.Id == "did:web:example.com#service-2");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Constructor_WithEmptyLists_ShouldCreateDidDocument()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };

        // Act
        var didDocument = new DidDocument
        {
            Id = id,
            Context = context,
            VerificationMethod = new List<VerificationMethod>(),
            Service = new List<ServiceEndpoint>()
        };

        // Assert
        didDocument.VerificationMethod.Should().BeEmpty();
        didDocument.Service.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithNullOptionalFields_ShouldCreateDidDocument()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };

        // Act
        var didDocument = new DidDocument
        {
            Id = id,
            Context = context,
            Controller = null,
            AlsoKnownAs = null,
            Authentication = null,
            AssertionMethod = null,
            KeyAgreement = null,
            CapabilityInvocation = null,
            CapabilityDelegation = null,
            VerificationMethod = null,
            Service = null
        };

        // Assert
        didDocument.Id.Should().Be(id);
        didDocument.Context.Should().BeEquivalentTo(context);
        didDocument.Controller.Should().BeNull();
        didDocument.AlsoKnownAs.Should().BeNull();
        didDocument.Authentication.Should().BeNull();
        didDocument.AssertionMethod.Should().BeNull();
        didDocument.KeyAgreement.Should().BeNull();
        didDocument.CapabilityInvocation.Should().BeNull();
        didDocument.CapabilityDelegation.Should().BeNull();
        didDocument.VerificationMethod.Should().BeNull();
        didDocument.Service.Should().BeNull();
    }

    [Fact]
    public void Equality_WithSameDidDocument_ShouldBeEqual()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };

        var didDocument1 = new DidDocument
        {
            Id = id,
            Context = context
        };

        var didDocument2 = new DidDocument
        {
            Id = id,
            Context = context
        };

        // Act & Assert
        didDocument1.Should().Be(didDocument2);
        didDocument1.GetHashCode().Should().Be(didDocument2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentDidDocument_ShouldNotBeEqual()
    {
        // Arrange
        var id = "did:web:example.com";
        var context = new List<string> { "https://www.w3.org/ns/did/v1" };

        var didDocument1 = new DidDocument
        {
            Id = id,
            Context = context
        };

        var didDocument2 = new DidDocument
        {
            Id = "did:web:different.com",
            Context = context
        };

        // Act & Assert
        didDocument1.Should().NotBe(didDocument2);
    }

    #endregion
}