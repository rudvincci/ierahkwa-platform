using FluentAssertions;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.VerifiableCredentials;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.VerifiableCredentials;

/// <summary>
/// Tests for the VerifiableCredential class covering W3C VC 1.1 compliance.
/// </summary>
public class VerifiableCredentialTests
{
    #region Happy Path Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateVerifiableCredential()
    {
        // Arrange
        var id = "https://example.com/credentials/123";
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiableCredential", "EmailCredential" };
        var issuer = "did:web:example.com";
        var issuanceDate = DateTime.UtcNow;
        var expirationDate = DateTime.UtcNow.AddDays(365);
        var credentialSubject = new CredentialSubject
        {
            Id = "did:web:user.com",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user@example.com",
                ["name"] = "John Doe"
            }
        };
        var proof = new Proof
        {
            Type = "Ed25519Signature2020",
            Created = DateTime.UtcNow,
            VerificationMethod = "did:web:example.com#key-1",
            ProofPurpose = "assertionMethod",
            ProofValue = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
        };

        // Act
        var verifiableCredential = new VerifiableCredential
        {
            Id = id,
            Context = context,
            Type = type,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            ExpirationDate = expirationDate,
            CredentialSubject = credentialSubject,
            Proof = proof
        };

        // Assert
        verifiableCredential.Id.Should().Be(id);
        verifiableCredential.Context.Should().BeEquivalentTo(context);
        verifiableCredential.Type.Should().BeEquivalentTo(type);
        verifiableCredential.Issuer.Should().Be(issuer);
        verifiableCredential.IssuanceDate.Should().Be(issuanceDate);
        verifiableCredential.ExpirationDate.Should().Be(expirationDate);
        verifiableCredential.CredentialSubject.Should().Be(credentialSubject);
        verifiableCredential.Proof.Should().Be(proof);
    }

    [Fact]
    public void Constructor_WithMinimalRequiredFields_ShouldCreateVerifiableCredential()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiableCredential" };
        var issuer = "did:web:example.com";
        var issuanceDate = DateTime.UtcNow;
        var credentialSubject = new CredentialSubject
        {
            Id = "did:web:user.com",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user@example.com"
            }
        };

        // Act
        var verifiableCredential = new VerifiableCredential
        {
            Context = context,
            Type = type,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            CredentialSubject = credentialSubject
        };

        // Assert
        verifiableCredential.Context.Should().BeEquivalentTo(context);
        verifiableCredential.Type.Should().BeEquivalentTo(type);
        verifiableCredential.Issuer.Should().Be(issuer);
        verifiableCredential.IssuanceDate.Should().Be(issuanceDate);
        verifiableCredential.CredentialSubject.Should().Be(credentialSubject);
    }

    [Fact]
    public void Constructor_WithMultipleTypes_ShouldCreateVerifiableCredential()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiableCredential", "EmailCredential", "IdentityCredential" };
        var issuer = "did:web:example.com";
        var issuanceDate = DateTime.UtcNow;
        var credentialSubject = new CredentialSubject
        {
            Id = "did:web:user.com",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user@example.com"
            }
        };

        // Act
        var verifiableCredential = new VerifiableCredential
        {
            Context = context,
            Type = type,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            CredentialSubject = credentialSubject
        };

        // Assert
        verifiableCredential.Type.Should().HaveCount(3);
        verifiableCredential.Type.Should().Contain("VerifiableCredential");
        verifiableCredential.Type.Should().Contain("EmailCredential");
        verifiableCredential.Type.Should().Contain("IdentityCredential");
    }

    [Fact]
    public void Constructor_WithMultipleContexts_ShouldCreateVerifiableCredential()
    {
        // Arrange
        var context = new List<string>
        {
            "https://www.w3.org/2018/credentials/v1",
            "https://w3id.org/security/suites/ed25519-2020/v1"
        };
        var type = new List<string> { "VerifiableCredential" };
        var issuer = "did:web:example.com";
        var issuanceDate = DateTime.UtcNow;
        var credentialSubject = new CredentialSubject
        {
            Id = "did:web:user.com",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user@example.com"
            }
        };

        // Act
        var verifiableCredential = new VerifiableCredential
        {
            Context = context,
            Type = type,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            CredentialSubject = credentialSubject
        };

        // Assert
        verifiableCredential.Context.Should().HaveCount(2);
        verifiableCredential.Context.Should().Contain("https://www.w3.org/2018/credentials/v1");
        verifiableCredential.Context.Should().Contain("https://w3id.org/security/suites/ed25519-2020/v1");
    }

    [Fact]
    public void Constructor_WithStatus_ShouldCreateVerifiableCredential()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiableCredential" };
        var issuer = "did:web:example.com";
        var issuanceDate = DateTime.UtcNow;
        var credentialSubject = new CredentialSubject
        {
            Id = "did:web:user.com",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user@example.com"
            }
        };
        var status = new Dictionary<string, object>
        {
            ["id"] = "https://example.com/status/123",
            ["type"] = "RevocationList2020Status",
            ["statusListIndex"] = "1",
            ["statusListCredential"] = "https://example.com/status/revocation-list"
        };

        // Act
        var verifiableCredential = new VerifiableCredential
        {
            Context = context,
            Type = type,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            CredentialSubject = credentialSubject,
            Status = status
        };

        // Assert
        verifiableCredential.Status.Should().BeEquivalentTo(status);
    }

    [Fact]
    public void Constructor_WithTermsOfUse_ShouldCreateVerifiableCredential()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiableCredential" };
        var issuer = "did:web:example.com";
        var issuanceDate = DateTime.UtcNow;
        var credentialSubject = new CredentialSubject
        {
            Id = "did:web:user.com",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user@example.com"
            }
        };
        var termsOfUse = new Dictionary<string, object>
        {
            ["id"] = "https://example.com/terms/123",
            ["type"] = "OdrlPolicy",
            ["permission"] = new Dictionary<string, object>
            {
                ["action"] = "use",
                ["target"] = "https://example.com/credentials/123"
            }
        };

        // Act
        var verifiableCredential = new VerifiableCredential
        {
            Context = context,
            Type = type,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            CredentialSubject = credentialSubject,
            TermsOfUse = termsOfUse
        };

        // Assert
        verifiableCredential.TermsOfUse.Should().BeEquivalentTo(termsOfUse);
    }

    #endregion

    #region Unhappy Path Tests

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-id")]
    [InlineData("not-a-uri")]
    public void Constructor_WithInvalidId_ShouldThrowArgumentException(string invalidId)
    {
        // Act & Assert
        var action = () => new VerifiableCredential
        {
            Id = invalidId,
            Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
            Type = new List<string> { "VerifiableCredential" },
            Issuer = "did:web:example.com",
            IssuanceDate = DateTime.UtcNow,
            CredentialSubject = new CredentialSubject
            {
                Id = "did:web:user.com",
                Properties = new Dictionary<string, object>
                {
                    ["email"] = "user@example.com"
                }
            }
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(new string[0])]
    public void Constructor_WithInvalidContext_ShouldThrowArgumentException(string[]? invalidContext)
    {
        // Act & Assert
        var action = () => new VerifiableCredential
        {
            Context = invalidContext?.ToList(),
            Type = new List<string> { "VerifiableCredential" },
            Issuer = "did:web:example.com",
            IssuanceDate = DateTime.UtcNow,
            CredentialSubject = new CredentialSubject
            {
                Id = "did:web:user.com",
                Properties = new Dictionary<string, object>
                {
                    ["email"] = "user@example.com"
                }
            }
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(new string[0])]
    public void Constructor_WithInvalidType_ShouldThrowArgumentException(string[]? invalidType)
    {
        // Act & Assert
        var action = () => new VerifiableCredential
        {
            Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
            Type = invalidType?.ToList(),
            Issuer = "did:web:example.com",
            IssuanceDate = DateTime.UtcNow,
            CredentialSubject = new CredentialSubject
            {
                Id = "did:web:user.com",
                Properties = new Dictionary<string, object>
                {
                    ["email"] = "user@example.com"
                }
            }
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-issuer")]
    [InlineData("not-a-did")]
    public void Constructor_WithInvalidIssuer_ShouldThrowArgumentException(string invalidIssuer)
    {
        // Act & Assert
        var action = () => new VerifiableCredential
        {
            Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
            Type = new List<string> { "VerifiableCredential" },
            Issuer = invalidIssuer,
            IssuanceDate = DateTime.UtcNow,
            CredentialSubject = new CredentialSubject
            {
                Id = "did:web:user.com",
                Properties = new Dictionary<string, object>
                {
                    ["email"] = "user@example.com"
                }
            }
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithInvalidIssuanceDate_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new VerifiableCredential
        {
            Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
            Type = new List<string> { "VerifiableCredential" },
            Issuer = "did:web:example.com",
            IssuanceDate = DateTime.MinValue,
            CredentialSubject = new CredentialSubject
            {
                Id = "did:web:user.com",
                Properties = new Dictionary<string, object>
                {
                    ["email"] = "user@example.com"
                }
            }
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithInvalidExpirationDate_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new VerifiableCredential
        {
            Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
            Type = new List<string> { "VerifiableCredential" },
            Issuer = "did:web:example.com",
            IssuanceDate = DateTime.UtcNow,
            ExpirationDate = DateTime.MinValue,
            CredentialSubject = new CredentialSubject
            {
                Id = "did:web:user.com",
                Properties = new Dictionary<string, object>
                {
                    ["email"] = "user@example.com"
                }
            }
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithNullCredentialSubject_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new VerifiableCredential
        {
            Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
            Type = new List<string> { "VerifiableCredential" },
            Issuer = "did:web:example.com",
            IssuanceDate = DateTime.UtcNow,
            CredentialSubject = null!
        };

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithExpirationDateBeforeIssuanceDate_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new VerifiableCredential
        {
            Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
            Type = new List<string> { "VerifiableCredential" },
            Issuer = "did:web:example.com",
            IssuanceDate = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddDays(-1),
            CredentialSubject = new CredentialSubject
            {
                Id = "did:web:user.com",
                Properties = new Dictionary<string, object>
                {
                    ["email"] = "user@example.com"
                }
            }
        };

        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region W3C VC 1.1 Compliance Tests

    [Fact]
    public void Constructor_WithW3CCompliantCredential_ShouldCreateVerifiableCredential()
    {
        // Arrange - Verifiable Credential compliant with W3C VC 1.1 spec
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiableCredential", "EmailCredential" };
        var issuer = "did:web:example.com";
        var issuanceDate = DateTime.UtcNow;
        var credentialSubject = new CredentialSubject
        {
            Id = "did:web:user.com",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user@example.com",
                ["name"] = "John Doe"
            }
        };
        var proof = new Proof
        {
            Type = "Ed25519Signature2020",
            Created = DateTime.UtcNow,
            VerificationMethod = "did:web:example.com#key-1",
            ProofPurpose = "assertionMethod",
            ProofValue = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
        };

        // Act
        var verifiableCredential = new VerifiableCredential
        {
            Context = context,
            Type = type,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            CredentialSubject = credentialSubject,
            Proof = proof
        };

        // Assert
        verifiableCredential.Context.Should().Contain("https://www.w3.org/2018/credentials/v1");
        verifiableCredential.Type.Should().Contain("VerifiableCredential");
        verifiableCredential.Issuer.Should().Be(issuer);
        verifiableCredential.IssuanceDate.Should().Be(issuanceDate);
        verifiableCredential.CredentialSubject.Should().Be(credentialSubject);
        verifiableCredential.Proof.Should().Be(proof);
    }

    [Fact]
    public void Constructor_WithEd25519Proof_ShouldCreateVerifiableCredential()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiableCredential" };
        var issuer = "did:web:example.com";
        var issuanceDate = DateTime.UtcNow;
        var credentialSubject = new CredentialSubject
        {
            Id = "did:web:user.com",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user@example.com"
            }
        };
        var proof = new Proof
        {
            Type = "Ed25519Signature2020",
            Created = DateTime.UtcNow,
            VerificationMethod = "did:web:example.com#key-1",
            ProofPurpose = "assertionMethod",
            ProofValue = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
        };

        // Act
        var verifiableCredential = new VerifiableCredential
        {
            Context = context,
            Type = type,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            CredentialSubject = credentialSubject,
            Proof = proof
        };

        // Assert
        verifiableCredential.Proof.Type.Should().Be("Ed25519Signature2020");
        verifiableCredential.Proof.ProofPurpose.Should().Be("assertionMethod");
    }

    [Fact]
    public void Constructor_WithRsaProof_ShouldCreateVerifiableCredential()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiableCredential" };
        var issuer = "did:web:example.com";
        var issuanceDate = DateTime.UtcNow;
        var credentialSubject = new CredentialSubject
        {
            Id = "did:web:user.com",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user@example.com"
            }
        };
        var proof = new Proof
        {
            Type = "RsaSignature2018",
            Created = DateTime.UtcNow,
            VerificationMethod = "did:web:example.com#key-1",
            ProofPurpose = "assertionMethod",
            ProofValue = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
        };

        // Act
        var verifiableCredential = new VerifiableCredential
        {
            Context = context,
            Type = type,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            CredentialSubject = credentialSubject,
            Proof = proof
        };

        // Assert
        verifiableCredential.Proof.Type.Should().Be("RsaSignature2018");
        verifiableCredential.Proof.ProofPurpose.Should().Be("assertionMethod");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Constructor_WithEmptyProperties_ShouldCreateVerifiableCredential()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiableCredential" };
        var issuer = "did:web:example.com";
        var issuanceDate = DateTime.UtcNow;
        var credentialSubject = new CredentialSubject
        {
            Id = "did:web:user.com",
            Properties = new Dictionary<string, object>()
        };

        // Act
        var verifiableCredential = new VerifiableCredential
        {
            Context = context,
            Type = type,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            CredentialSubject = credentialSubject
        };

        // Assert
        verifiableCredential.CredentialSubject.Properties.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithNullOptionalFields_ShouldCreateVerifiableCredential()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiableCredential" };
        var issuer = "did:web:example.com";
        var issuanceDate = DateTime.UtcNow;
        var credentialSubject = new CredentialSubject
        {
            Id = "did:web:user.com",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user@example.com"
            }
        };

        // Act
        var verifiableCredential = new VerifiableCredential
        {
            Id = null,
            Context = context,
            Type = type,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            ExpirationDate = null,
            CredentialSubject = credentialSubject,
            Proof = null,
            Status = null,
            TermsOfUse = null
        };

        // Assert
        verifiableCredential.Id.Should().BeNull();
        verifiableCredential.ExpirationDate.Should().BeNull();
        verifiableCredential.Proof.Should().BeNull();
        verifiableCredential.Status.Should().BeNull();
        verifiableCredential.TermsOfUse.Should().BeNull();
    }

    [Fact]
    public void Equality_WithSameCredential_ShouldBeEqual()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiableCredential" };
        var issuer = "did:web:example.com";
        var issuanceDate = DateTime.UtcNow;
        var credentialSubject = new CredentialSubject
        {
            Id = "did:web:user.com",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user@example.com"
            }
        };

        var credential1 = new VerifiableCredential
        {
            Context = context,
            Type = type,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            CredentialSubject = credentialSubject
        };

        var credential2 = new VerifiableCredential
        {
            Context = context,
            Type = type,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            CredentialSubject = credentialSubject
        };

        // Act & Assert
        credential1.Should().Be(credential2);
        credential1.GetHashCode().Should().Be(credential2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentCredential_ShouldNotBeEqual()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiableCredential" };
        var issuer = "did:web:example.com";
        var issuanceDate = DateTime.UtcNow;
        var credentialSubject1 = new CredentialSubject
        {
            Id = "did:web:user1.com",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user1@example.com"
            }
        };
        var credentialSubject2 = new CredentialSubject
        {
            Id = "did:web:user2.com",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user2@example.com"
            }
        };

        var credential1 = new VerifiableCredential
        {
            Context = context,
            Type = type,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            CredentialSubject = credentialSubject1
        };

        var credential2 = new VerifiableCredential
        {
            Context = context,
            Type = type,
            Issuer = issuer,
            IssuanceDate = issuanceDate,
            CredentialSubject = credentialSubject2
        };

        // Act & Assert
        credential1.Should().NotBe(credential2);
    }

    #endregion
}
