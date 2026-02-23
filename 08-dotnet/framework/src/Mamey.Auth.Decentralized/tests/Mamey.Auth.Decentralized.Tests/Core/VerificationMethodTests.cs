using FluentAssertions;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Exceptions;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.Core;

/// <summary>
/// Tests for the VerificationMethod class covering W3C DID 1.1 compliance.
/// </summary>
public class VerificationMethodTests
{
    #region Happy Path Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateVerificationMethod()
    {
        // Arrange
        var id = "did:web:example.com#key-1";
        var type = "Ed25519VerificationKey2020";
        var controller = "did:web:example.com";
        var publicKeyJwk = new Dictionary<string, object>
        {
            ["kty"] = "OKP",
            ["crv"] = "Ed25519",
            ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
        };

        // Act
        var verificationMethod = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyJwk = publicKeyJwk
        };

        // Assert
        verificationMethod.Id.Should().Be(id);
        verificationMethod.Type.Should().Be(type);
        verificationMethod.Controller.Should().Be(controller);
        verificationMethod.PublicKeyJwk.Should().BeEquivalentTo(publicKeyJwk);
    }

    [Fact]
    public void Constructor_WithPublicKeyMultibase_ShouldCreateVerificationMethod()
    {
        // Arrange
        var id = "did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw#key-1";
        var type = "Ed25519VerificationKey2020";
        var controller = "did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw";
        var publicKeyMultibase = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw";

        // Act
        var verificationMethod = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyMultibase = publicKeyMultibase
        };

        // Assert
        verificationMethod.Id.Should().Be(id);
        verificationMethod.Type.Should().Be(type);
        verificationMethod.Controller.Should().Be(controller);
        verificationMethod.PublicKeyMultibase.Should().Be(publicKeyMultibase);
    }

    [Fact]
    public void Constructor_WithBothPublicKeyFormats_ShouldCreateVerificationMethod()
    {
        // Arrange
        var id = "did:web:example.com#key-1";
        var type = "Ed25519VerificationKey2020";
        var controller = "did:web:example.com";
        var publicKeyJwk = new Dictionary<string, object>
        {
            ["kty"] = "OKP",
            ["crv"] = "Ed25519",
            ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
        };
        var publicKeyMultibase = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw";

        // Act
        var verificationMethod = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyJwk = publicKeyJwk,
            PublicKeyMultibase = publicKeyMultibase
        };

        // Assert
        verificationMethod.Id.Should().Be(id);
        verificationMethod.Type.Should().Be(type);
        verificationMethod.Controller.Should().Be(controller);
        verificationMethod.PublicKeyJwk.Should().BeEquivalentTo(publicKeyJwk);
        verificationMethod.PublicKeyMultibase.Should().Be(publicKeyMultibase);
    }

    [Theory]
    [InlineData("Ed25519VerificationKey2020")]
    [InlineData("Ed25519VerificationKey2018")]
    [InlineData("JsonWebKey2020")]
    [InlineData("EcdsaSecp256k1VerificationKey2019")]
    [InlineData("EcdsaSecp256k1RecoveryMethod2020")]
    [InlineData("RsaVerificationKey2018")]
    public void Constructor_WithSupportedVerificationMethodTypes_ShouldCreateVerificationMethod(string type)
    {
        // Arrange
        var id = "did:web:example.com#key-1";
        var controller = "did:web:example.com";
        var publicKeyJwk = new Dictionary<string, object>
        {
            ["kty"] = "OKP",
            ["crv"] = "Ed25519",
            ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
        };

        // Act
        var verificationMethod = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyJwk = publicKeyJwk
        };

        // Assert
        verificationMethod.Type.Should().Be(type);
    }

    [Fact]
    public void Constructor_WithValidJwk_ShouldCreateVerificationMethod()
    {
        // Arrange
        var id = "did:web:example.com#key-1";
        var type = "Ed25519VerificationKey2020";
        var controller = "did:web:example.com";
        var publicKeyJwk = new Dictionary<string, object>
        {
            ["kty"] = "OKP",
            ["crv"] = "Ed25519",
            ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw",
            ["use"] = "sig",
            ["alg"] = "EdDSA"
        };

        // Act
        var verificationMethod = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyJwk = publicKeyJwk
        };

        // Assert
        verificationMethod.PublicKeyJwk.Should().BeEquivalentTo(publicKeyJwk);
    }

    [Fact]
    public void Constructor_WithValidMultibase_ShouldCreateVerificationMethod()
    {
        // Arrange
        var id = "did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw#key-1";
        var type = "Ed25519VerificationKey2020";
        var controller = "did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw";
        var publicKeyMultibase = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw";

        // Act
        var verificationMethod = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyMultibase = publicKeyMultibase
        };

        // Assert
        verificationMethod.PublicKeyMultibase.Should().Be(publicKeyMultibase);
    }

    #endregion

    #region Unhappy Path Tests

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-id")]
    [InlineData("not-a-did-url")]
    public void Constructor_WithInvalidId_ShouldThrowInvalidDidException(string invalidId)
    {
        // Act & Assert
        var action = () => new VerificationMethod
        {
            Id = invalidId,
            Type = "Ed25519VerificationKey2020",
            Controller = "did:web:example.com",
            PublicKeyJwk = new Dictionary<string, object>
            {
                ["kty"] = "OKP",
                ["crv"] = "Ed25519",
                ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
            }
        };

        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-type")]
    public void Constructor_WithInvalidType_ShouldThrowArgumentException(string invalidType)
    {
        // Act & Assert
        var action = () => new VerificationMethod
        {
            Id = "did:web:example.com#key-1",
            Type = invalidType,
            Controller = "did:web:example.com",
            PublicKeyJwk = new Dictionary<string, object>
            {
                ["kty"] = "OKP",
                ["crv"] = "Ed25519",
                ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
            }
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-controller")]
    [InlineData("not-a-did")]
    public void Constructor_WithInvalidController_ShouldThrowInvalidDidException(string invalidController)
    {
        // Act & Assert
        var action = () => new VerificationMethod
        {
            Id = "did:web:example.com#key-1",
            Type = "Ed25519VerificationKey2020",
            Controller = invalidController,
            PublicKeyJwk = new Dictionary<string, object>
            {
                ["kty"] = "OKP",
                ["crv"] = "Ed25519",
                ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
            }
        };

        action.Should().Throw<InvalidDidException>();
    }

    [Fact]
    public void Constructor_WithoutPublicKeyMaterial_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new VerificationMethod
        {
            Id = "did:web:example.com#key-1",
            Type = "Ed25519VerificationKey2020",
            Controller = "did:web:example.com"
            // No public key material provided
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithEmptyJwk_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new VerificationMethod
        {
            Id = "did:web:example.com#key-1",
            Type = "Ed25519VerificationKey2020",
            Controller = "did:web:example.com",
            PublicKeyJwk = new Dictionary<string, object>()
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-multibase")]
    [InlineData("not-multibase")]
    public void Constructor_WithInvalidMultibase_ShouldThrowArgumentException(string invalidMultibase)
    {
        // Act & Assert
        var action = () => new VerificationMethod
        {
            Id = "did:web:example.com#key-1",
            Type = "Ed25519VerificationKey2020",
            Controller = "did:web:example.com",
            PublicKeyMultibase = invalidMultibase
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithInvalidJwkFormat_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidJwk = new Dictionary<string, object>
        {
            ["invalid"] = "jwk"
            // Missing required fields like kty, crv, etc.
        };

        // Act & Assert
        var action = () => new VerificationMethod
        {
            Id = "did:web:example.com#key-1",
            Type = "Ed25519VerificationKey2020",
            Controller = "did:web:example.com",
            PublicKeyJwk = invalidJwk
        };

        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region W3C DID 1.1 Compliance Tests

    [Fact]
    public void Constructor_WithW3CCompliantJwk_ShouldCreateVerificationMethod()
    {
        // Arrange - JWK compliant with W3C DID 1.1 spec
        var id = "did:web:example.com#key-1";
        var type = "Ed25519VerificationKey2020";
        var controller = "did:web:example.com";
        var publicKeyJwk = new Dictionary<string, object>
        {
            ["kty"] = "OKP",
            ["crv"] = "Ed25519",
            ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw",
            ["use"] = "sig",
            ["alg"] = "EdDSA"
        };

        // Act
        var verificationMethod = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyJwk = publicKeyJwk
        };

        // Assert
        verificationMethod.PublicKeyJwk.Should().ContainKey("kty");
        verificationMethod.PublicKeyJwk.Should().ContainKey("crv");
        verificationMethod.PublicKeyJwk.Should().ContainKey("x");
    }

    [Fact]
    public void Constructor_WithW3CCompliantMultibase_ShouldCreateVerificationMethod()
    {
        // Arrange - Multibase compliant with W3C DID 1.1 spec
        var id = "did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw#key-1";
        var type = "Ed25519VerificationKey2020";
        var controller = "did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw";
        var publicKeyMultibase = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw";

        // Act
        var verificationMethod = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyMultibase = publicKeyMultibase
        };

        // Assert
        verificationMethod.PublicKeyMultibase.Should().StartWith("z");
    }

    [Fact]
    public void Constructor_WithRsaJwk_ShouldCreateVerificationMethod()
    {
        // Arrange - RSA JWK compliant with W3C DID 1.1 spec
        var id = "did:web:example.com#key-1";
        var type = "RsaVerificationKey2018";
        var controller = "did:web:example.com";
        var publicKeyJwk = new Dictionary<string, object>
        {
            ["kty"] = "RSA",
            ["n"] = "n-value",
            ["e"] = "AQAB",
            ["use"] = "sig",
            ["alg"] = "RS256"
        };

        // Act
        var verificationMethod = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyJwk = publicKeyJwk
        };

        // Assert
        verificationMethod.PublicKeyJwk.Should().ContainKey("kty");
        verificationMethod.PublicKeyJwk.Should().ContainKey("n");
        verificationMethod.PublicKeyJwk.Should().ContainKey("e");
    }

    [Fact]
    public void Constructor_WithSecp256k1Jwk_ShouldCreateVerificationMethod()
    {
        // Arrange - Secp256k1 JWK compliant with W3C DID 1.1 spec
        var id = "did:web:example.com#key-1";
        var type = "EcdsaSecp256k1VerificationKey2019";
        var controller = "did:web:example.com";
        var publicKeyJwk = new Dictionary<string, object>
        {
            ["kty"] = "EC",
            ["crv"] = "secp256k1",
            ["x"] = "x-value",
            ["y"] = "y-value",
            ["use"] = "sig",
            ["alg"] = "ES256K"
        };

        // Act
        var verificationMethod = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyJwk = publicKeyJwk
        };

        // Assert
        verificationMethod.PublicKeyJwk.Should().ContainKey("kty");
        verificationMethod.PublicKeyJwk.Should().ContainKey("crv");
        verificationMethod.PublicKeyJwk.Should().ContainKey("x");
        verificationMethod.PublicKeyJwk.Should().ContainKey("y");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Constructor_WithSpecialCharactersInId_ShouldCreateVerificationMethod()
    {
        // Arrange
        var id = "did:web:example.com#key-with-special-chars_123";
        var type = "Ed25519VerificationKey2020";
        var controller = "did:web:example.com";
        var publicKeyJwk = new Dictionary<string, object>
        {
            ["kty"] = "OKP",
            ["crv"] = "Ed25519",
            ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
        };

        // Act
        var verificationMethod = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyJwk = publicKeyJwk
        };

        // Assert
        verificationMethod.Id.Should().Be(id);
    }

    [Fact]
    public void Constructor_WithLongJwk_ShouldCreateVerificationMethod()
    {
        // Arrange
        var id = "did:web:example.com#key-1";
        var type = "Ed25519VerificationKey2020";
        var controller = "did:web:example.com";
        var publicKeyJwk = new Dictionary<string, object>
        {
            ["kty"] = "OKP",
            ["crv"] = "Ed25519",
            ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw",
            ["use"] = "sig",
            ["alg"] = "EdDSA",
            ["kid"] = "key-1",
            ["x5u"] = "https://example.com/cert.pem",
            ["x5c"] = "certificate-chain",
            ["x5t"] = "thumbprint",
            ["x5t#S256"] = "sha256-thumbprint"
        };

        // Act
        var verificationMethod = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyJwk = publicKeyJwk
        };

        // Assert
        verificationMethod.PublicKeyJwk.Should().BeEquivalentTo(publicKeyJwk);
    }

    [Fact]
    public void Constructor_WithNestedJwk_ShouldCreateVerificationMethod()
    {
        // Arrange
        var id = "did:web:example.com#key-1";
        var type = "Ed25519VerificationKey2020";
        var controller = "did:web:example.com";
        var publicKeyJwk = new Dictionary<string, object>
        {
            ["kty"] = "OKP",
            ["crv"] = "Ed25519",
            ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw",
            ["use"] = "sig",
            ["alg"] = "EdDSA",
            ["key_ops"] = new[] { "sign", "verify" },
            ["ext"] = true,
            ["key_use"] = "sig"
        };

        // Act
        var verificationMethod = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyJwk = publicKeyJwk
        };

        // Assert
        verificationMethod.PublicKeyJwk.Should().BeEquivalentTo(publicKeyJwk);
    }

    [Fact]
    public void Equality_WithSameVerificationMethod_ShouldBeEqual()
    {
        // Arrange
        var id = "did:web:example.com#key-1";
        var type = "Ed25519VerificationKey2020";
        var controller = "did:web:example.com";
        var publicKeyJwk = new Dictionary<string, object>
        {
            ["kty"] = "OKP",
            ["crv"] = "Ed25519",
            ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
        };

        var verificationMethod1 = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyJwk = publicKeyJwk
        };

        var verificationMethod2 = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyJwk = publicKeyJwk
        };

        // Act & Assert
        verificationMethod1.Should().Be(verificationMethod2);
        verificationMethod1.GetHashCode().Should().Be(verificationMethod2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentVerificationMethod_ShouldNotBeEqual()
    {
        // Arrange
        var id = "did:web:example.com#key-1";
        var type = "Ed25519VerificationKey2020";
        var controller = "did:web:example.com";
        var publicKeyJwk = new Dictionary<string, object>
        {
            ["kty"] = "OKP",
            ["crv"] = "Ed25519",
            ["x"] = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
        };

        var verificationMethod1 = new VerificationMethod
        {
            Id = id,
            Type = type,
            Controller = controller,
            PublicKeyJwk = publicKeyJwk
        };

        var verificationMethod2 = new VerificationMethod
        {
            Id = "did:web:example.com#key-2",
            Type = type,
            Controller = controller,
            PublicKeyJwk = publicKeyJwk
        };

        // Act & Assert
        verificationMethod1.Should().NotBe(verificationMethod2);
    }

    #endregion
}
