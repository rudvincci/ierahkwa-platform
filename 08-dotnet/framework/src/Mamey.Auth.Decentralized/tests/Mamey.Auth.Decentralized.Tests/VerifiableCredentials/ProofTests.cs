using FluentAssertions;
using Mamey.Auth.Decentralized.VerifiableCredentials;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.VerifiableCredentials;

/// <summary>
/// Tests for the Proof class covering W3C VC 1.1 compliance.
/// </summary>
public class ProofTests
{
    #region Happy Path Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateProof()
    {
        // Arrange
        var type = "Ed25519Signature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "assertionMethod";
        var proofValue = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw";

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            ProofValue = proofValue
        };

        // Assert
        proof.Type.Should().Be(type);
        proof.Created.Should().Be(created);
        proof.VerificationMethod.Should().Be(verificationMethod);
        proof.ProofPurpose.Should().Be(proofPurpose);
        proof.ProofValue.Should().Be(proofValue);
    }

    [Fact]
    public void Constructor_WithMinimalRequiredFields_ShouldCreateProof()
    {
        // Arrange
        var type = "Ed25519Signature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "assertionMethod";

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose
        };

        // Assert
        proof.Type.Should().Be(type);
        proof.Created.Should().Be(created);
        proof.VerificationMethod.Should().Be(verificationMethod);
        proof.ProofPurpose.Should().Be(proofPurpose);
    }

    [Theory]
    [InlineData("Ed25519Signature2020")]
    [InlineData("Ed25519Signature2018")]
    [InlineData("RsaSignature2018")]
    [InlineData("EcdsaSecp256k1Signature2019")]
    [InlineData("JsonWebSignature2020")]
    public void Constructor_WithSupportedProofTypes_ShouldCreateProof(string type)
    {
        // Arrange
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "assertionMethod";

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose
        };

        // Assert
        proof.Type.Should().Be(type);
    }

    [Theory]
    [InlineData("assertionMethod")]
    [InlineData("authentication")]
    [InlineData("keyAgreement")]
    [InlineData("capabilityInvocation")]
    [InlineData("capabilityDelegation")]
    public void Constructor_WithSupportedProofPurposes_ShouldCreateProof(string proofPurpose)
    {
        // Arrange
        var type = "Ed25519Signature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose
        };

        // Assert
        proof.ProofPurpose.Should().Be(proofPurpose);
    }

    [Fact]
    public void Constructor_WithJws_ShouldCreateProof()
    {
        // Arrange
        var type = "JsonWebSignature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "assertionMethod";
        var jws = "eyJhbGciOiJFZERTQSIsImtpZCI6ImRpZDp3ZWI6ZXhhbXBsZS5jb20ja2V5LTEifQ.eyJpc3MiOiJkaWQ6d2ViOmV4YW1wbGUuY29tIiwic3ViIjoiZGlkOndlYjp1c2VyLmNvbSIsIm5iZiI6MTYwOTQ1MjAwMCwiZXhwIjoxNjA5NTM4NDAwfQ.signature";

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            Jws = jws
        };

        // Assert
        proof.Jws.Should().Be(jws);
    }

    [Fact]
    public void Constructor_WithChallenge_ShouldCreateProof()
    {
        // Arrange
        var type = "Ed25519Signature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "authentication";
        var challenge = "challenge-123";

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            Challenge = challenge
        };

        // Assert
        proof.Challenge.Should().Be(challenge);
    }

    [Fact]
    public void Constructor_WithDomain_ShouldCreateProof()
    {
        // Arrange
        var type = "Ed25519Signature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "authentication";
        var domain = "example.com";

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            Domain = domain
        };

        // Assert
        proof.Domain.Should().Be(domain);
    }

    [Fact]
    public void Constructor_WithNonce_ShouldCreateProof()
    {
        // Arrange
        var type = "Ed25519Signature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "assertionMethod";
        var nonce = "nonce-123";

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            Nonce = nonce
        };

        // Assert
        proof.Nonce.Should().Be(nonce);
    }

    #endregion

    #region Unhappy Path Tests

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-type")]
    public void Constructor_WithInvalidType_ShouldThrowArgumentException(string invalidType)
    {
        // Act & Assert
        var action = () => new Proof
        {
            Type = invalidType,
            Created = DateTime.UtcNow,
            VerificationMethod = "did:web:example.com#key-1",
            ProofPurpose = "assertionMethod"
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithInvalidCreated_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new Proof
        {
            Type = "Ed25519Signature2020",
            Created = DateTime.MinValue,
            VerificationMethod = "did:web:example.com#key-1",
            ProofPurpose = "assertionMethod"
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-verification-method")]
    [InlineData("not-a-did-url")]
    public void Constructor_WithInvalidVerificationMethod_ShouldThrowArgumentException(string invalidVerificationMethod)
    {
        // Act & Assert
        var action = () => new Proof
        {
            Type = "Ed25519Signature2020",
            Created = DateTime.UtcNow,
            VerificationMethod = invalidVerificationMethod,
            ProofPurpose = "assertionMethod"
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-purpose")]
    public void Constructor_WithInvalidProofPurpose_ShouldThrowArgumentException(string invalidProofPurpose)
    {
        // Act & Assert
        var action = () => new Proof
        {
            Type = "Ed25519Signature2020",
            Created = DateTime.UtcNow,
            VerificationMethod = "did:web:example.com#key-1",
            ProofPurpose = invalidProofPurpose
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-proof-value")]
    public void Constructor_WithInvalidProofValue_ShouldThrowArgumentException(string invalidProofValue)
    {
        // Act & Assert
        var action = () => new Proof
        {
            Type = "Ed25519Signature2020",
            Created = DateTime.UtcNow,
            VerificationMethod = "did:web:example.com#key-1",
            ProofPurpose = "assertionMethod",
            ProofValue = invalidProofValue
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-jws")]
    public void Constructor_WithInvalidJws_ShouldThrowArgumentException(string invalidJws)
    {
        // Act & Assert
        var action = () => new Proof
        {
            Type = "JsonWebSignature2020",
            Created = DateTime.UtcNow,
            VerificationMethod = "did:web:example.com#key-1",
            ProofPurpose = "assertionMethod",
            Jws = invalidJws
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-challenge")]
    public void Constructor_WithInvalidChallenge_ShouldThrowArgumentException(string invalidChallenge)
    {
        // Act & Assert
        var action = () => new Proof
        {
            Type = "Ed25519Signature2020",
            Created = DateTime.UtcNow,
            VerificationMethod = "did:web:example.com#key-1",
            ProofPurpose = "authentication",
            Challenge = invalidChallenge
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-domain")]
    public void Constructor_WithInvalidDomain_ShouldThrowArgumentException(string invalidDomain)
    {
        // Act & Assert
        var action = () => new Proof
        {
            Type = "Ed25519Signature2020",
            Created = DateTime.UtcNow,
            VerificationMethod = "did:web:example.com#key-1",
            ProofPurpose = "authentication",
            Domain = invalidDomain
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-nonce")]
    public void Constructor_WithInvalidNonce_ShouldThrowArgumentException(string invalidNonce)
    {
        // Act & Assert
        var action = () => new Proof
        {
            Type = "Ed25519Signature2020",
            Created = DateTime.UtcNow,
            VerificationMethod = "did:web:example.com#key-1",
            ProofPurpose = "assertionMethod",
            Nonce = invalidNonce
        };

        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region W3C VC 1.1 Compliance Tests

    [Fact]
    public void Constructor_WithW3CCompliantProof_ShouldCreateProof()
    {
        // Arrange - Proof compliant with W3C VC 1.1 spec
        var type = "Ed25519Signature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "assertionMethod";
        var proofValue = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw";

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            ProofValue = proofValue
        };

        // Assert
        proof.Type.Should().Be(type);
        proof.Created.Should().Be(created);
        proof.VerificationMethod.Should().Be(verificationMethod);
        proof.ProofPurpose.Should().Be(proofPurpose);
        proof.ProofValue.Should().Be(proofValue);
    }

    [Fact]
    public void Constructor_WithEd25519Signature2020_ShouldCreateProof()
    {
        // Arrange
        var type = "Ed25519Signature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "assertionMethod";
        var proofValue = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw";

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            ProofValue = proofValue
        };

        // Assert
        proof.Type.Should().Be("Ed25519Signature2020");
        proof.ProofValue.Should().Be(proofValue);
    }

    [Fact]
    public void Constructor_WithRsaSignature2018_ShouldCreateProof()
    {
        // Arrange
        var type = "RsaSignature2018";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "assertionMethod";
        var proofValue = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw";

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            ProofValue = proofValue
        };

        // Assert
        proof.Type.Should().Be("RsaSignature2018");
        proof.ProofValue.Should().Be(proofValue);
    }

    [Fact]
    public void Constructor_WithJsonWebSignature2020_ShouldCreateProof()
    {
        // Arrange
        var type = "JsonWebSignature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "assertionMethod";
        var jws = "eyJhbGciOiJFZERTQSIsImtpZCI6ImRpZDp3ZWI6ZXhhbXBsZS5jb20ja2V5LTEifQ.eyJpc3MiOiJkaWQ6d2ViOmV4YW1wbGUuY29tIiwic3ViIjoiZGlkOndlYjp1c2VyLmNvbSIsIm5iZiI6MTYwOTQ1MjAwMCwiZXhwIjoxNjA5NTM4NDAwfQ.signature";

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            Jws = jws
        };

        // Assert
        proof.Type.Should().Be("JsonWebSignature2020");
        proof.Jws.Should().Be(jws);
    }

    [Fact]
    public void Constructor_WithAuthenticationProof_ShouldCreateProof()
    {
        // Arrange
        var type = "Ed25519Signature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "authentication";
        var challenge = "challenge-123";
        var domain = "example.com";

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            Challenge = challenge,
            Domain = domain
        };

        // Assert
        proof.ProofPurpose.Should().Be("authentication");
        proof.Challenge.Should().Be(challenge);
        proof.Domain.Should().Be(domain);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Constructor_WithNullOptionalFields_ShouldCreateProof()
    {
        // Arrange
        var type = "Ed25519Signature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "assertionMethod";

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            ProofValue = null,
            Jws = null,
            Challenge = null,
            Domain = null,
            Nonce = null
        };

        // Assert
        proof.Type.Should().Be(type);
        proof.Created.Should().Be(created);
        proof.VerificationMethod.Should().Be(verificationMethod);
        proof.ProofPurpose.Should().Be(proofPurpose);
        proof.ProofValue.Should().BeNull();
        proof.Jws.Should().BeNull();
        proof.Challenge.Should().BeNull();
        proof.Domain.Should().BeNull();
        proof.Nonce.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithLongProofValue_ShouldCreateProof()
    {
        // Arrange
        var type = "Ed25519Signature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "assertionMethod";
        var longProofValue = new string('A', 1000);

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            ProofValue = longProofValue
        };

        // Assert
        proof.ProofValue.Should().Be(longProofValue);
    }

    [Fact]
    public void Constructor_WithSpecialCharactersInProofValue_ShouldCreateProof()
    {
        // Arrange
        var type = "Ed25519Signature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "assertionMethod";
        var proofValueWithSpecialChars = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw!@#$%^&*()";

        // Act
        var proof = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            ProofValue = proofValueWithSpecialChars
        };

        // Assert
        proof.ProofValue.Should().Be(proofValueWithSpecialChars);
    }

    [Fact]
    public void Equality_WithSameProof_ShouldBeEqual()
    {
        // Arrange
        var type = "Ed25519Signature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "assertionMethod";
        var proofValue = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw";

        var proof1 = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            ProofValue = proofValue
        };

        var proof2 = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            ProofValue = proofValue
        };

        // Act & Assert
        proof1.Should().Be(proof2);
        proof1.GetHashCode().Should().Be(proof2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentProof_ShouldNotBeEqual()
    {
        // Arrange
        var type = "Ed25519Signature2020";
        var created = DateTime.UtcNow;
        var verificationMethod = "did:web:example.com#key-1";
        var proofPurpose = "assertionMethod";
        var proofValue1 = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw";
        var proofValue2 = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw2";

        var proof1 = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            ProofValue = proofValue1
        };

        var proof2 = new Proof
        {
            Type = type,
            Created = created,
            VerificationMethod = verificationMethod,
            ProofPurpose = proofPurpose,
            ProofValue = proofValue2
        };

        // Act & Assert
        proof1.Should().NotBe(proof2);
    }

    #endregion
}
