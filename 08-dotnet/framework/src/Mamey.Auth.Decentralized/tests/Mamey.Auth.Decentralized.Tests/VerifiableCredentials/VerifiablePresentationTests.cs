using FluentAssertions;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.VerifiableCredentials;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.VerifiableCredentials;

/// <summary>
/// Tests for the VerifiablePresentation class covering W3C VC 1.1 compliance.
/// </summary>
public class VerifiablePresentationTests
{
    #region Happy Path Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateVerifiablePresentation()
    {
        // Arrange
        var id = "https://example.com/presentations/123";
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiablePresentation" };
        var holder = "did:web:user.com";
        var verifiableCredential = new VerifiableCredential
        {
            Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
            Type = new List<string> { "VerifiableCredential", "EmailCredential" },
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
        var proof = new Proof
        {
            Type = "Ed25519Signature2020",
            Created = DateTime.UtcNow,
            VerificationMethod = "did:web:user.com#key-1",
            ProofPurpose = "authentication",
            ProofValue = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
        };

        // Act
        var verifiablePresentation = new VerifiablePresentation
        {
            Id = id,
            Context = context,
            Type = type,
            Holder = holder,
            VerifiableCredential = new List<VerifiableCredential> { verifiableCredential },
            Proof = proof
        };

        // Assert
        verifiablePresentation.Id.Should().Be(id);
        verifiablePresentation.Context.Should().BeEquivalentTo(context);
        verifiablePresentation.Type.Should().BeEquivalentTo(type);
        verifiablePresentation.Holder.Should().Be(holder);
        verifiablePresentation.VerifiableCredential.Should().Contain(verifiableCredential);
        verifiablePresentation.Proof.Should().Be(proof);
    }

    [Fact]
    public void Constructor_WithMinimalRequiredFields_ShouldCreateVerifiablePresentation()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiablePresentation" };
        var holder = "did:web:user.com";
        var verifiableCredential = new VerifiableCredential
        {
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

        // Act
        var verifiablePresentation = new VerifiablePresentation
        {
            Context = context,
            Type = type,
            Holder = holder,
            VerifiableCredential = new List<VerifiableCredential> { verifiableCredential }
        };

        // Assert
        verifiablePresentation.Context.Should().BeEquivalentTo(context);
        verifiablePresentation.Type.Should().BeEquivalentTo(type);
        verifiablePresentation.Holder.Should().Be(holder);
        verifiablePresentation.VerifiableCredential.Should().Contain(verifiableCredential);
    }

    [Fact]
    public void Constructor_WithMultipleCredentials_ShouldCreateVerifiablePresentation()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiablePresentation" };
        var holder = "did:web:user.com";
        var credentials = new List<VerifiableCredential>
        {
            new()
            {
                Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
                Type = new List<string> { "VerifiableCredential", "EmailCredential" },
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
            },
            new()
            {
                Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
                Type = new List<string> { "VerifiableCredential", "IdentityCredential" },
                Issuer = "did:web:example.com",
                IssuanceDate = DateTime.UtcNow,
                CredentialSubject = new CredentialSubject
                {
                    Id = "did:web:user.com",
                    Properties = new Dictionary<string, object>
                    {
                        ["name"] = "John Doe"
                    }
                }
            }
        };

        // Act
        var verifiablePresentation = new VerifiablePresentation
        {
            Context = context,
            Type = type,
            Holder = holder,
            VerifiableCredential = credentials
        };

        // Assert
        verifiablePresentation.VerifiableCredential.Should().HaveCount(2);
        verifiablePresentation.VerifiableCredential.Should().Contain(c => c.Type.Contains("EmailCredential"));
        verifiablePresentation.VerifiableCredential.Should().Contain(c => c.Type.Contains("IdentityCredential"));
    }

    [Fact]
    public void Constructor_WithMultipleTypes_ShouldCreateVerifiablePresentation()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiablePresentation", "IdentityPresentation" };
        var holder = "did:web:user.com";
        var verifiableCredential = new VerifiableCredential
        {
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

        // Act
        var verifiablePresentation = new VerifiablePresentation
        {
            Context = context,
            Type = type,
            Holder = holder,
            VerifiableCredential = new List<VerifiableCredential> { verifiableCredential }
        };

        // Assert
        verifiablePresentation.Type.Should().HaveCount(2);
        verifiablePresentation.Type.Should().Contain("VerifiablePresentation");
        verifiablePresentation.Type.Should().Contain("IdentityPresentation");
    }

    [Fact]
    public void Constructor_WithMultipleContexts_ShouldCreateVerifiablePresentation()
    {
        // Arrange
        var context = new List<string>
        {
            "https://www.w3.org/2018/credentials/v1",
            "https://w3id.org/security/suites/ed25519-2020/v1"
        };
        var type = new List<string> { "VerifiablePresentation" };
        var holder = "did:web:user.com";
        var verifiableCredential = new VerifiableCredential
        {
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

        // Act
        var verifiablePresentation = new VerifiablePresentation
        {
            Context = context,
            Type = type,
            Holder = holder,
            VerifiableCredential = new List<VerifiableCredential> { verifiableCredential }
        };

        // Assert
        verifiablePresentation.Context.Should().HaveCount(2);
        verifiablePresentation.Context.Should().Contain("https://www.w3.org/2018/credentials/v1");
        verifiablePresentation.Context.Should().Contain("https://w3id.org/security/suites/ed25519-2020/v1");
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
        var action = () => new VerifiablePresentation
        {
            Id = invalidId,
            Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
            Type = new List<string> { "VerifiablePresentation" },
            Holder = "did:web:user.com",
            VerifiableCredential = new List<VerifiableCredential>
            {
                new()
                {
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
        var action = () => new VerifiablePresentation
        {
            Context = invalidContext?.ToList(),
            Type = new List<string> { "VerifiablePresentation" },
            Holder = "did:web:user.com",
            VerifiableCredential = new List<VerifiableCredential>
            {
                new()
                {
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
        var action = () => new VerifiablePresentation
        {
            Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
            Type = invalidType?.ToList(),
            Holder = "did:web:user.com",
            VerifiableCredential = new List<VerifiableCredential>
            {
                new()
                {
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
                }
            }
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-holder")]
    [InlineData("not-a-did")]
    public void Constructor_WithInvalidHolder_ShouldThrowArgumentException(string invalidHolder)
    {
        // Act & Assert
        var action = () => new VerifiablePresentation
        {
            Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
            Type = new List<string> { "VerifiablePresentation" },
            Holder = invalidHolder,
            VerifiableCredential = new List<VerifiableCredential>
            {
                new()
                {
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
                }
            }
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithNullVerifiableCredential_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new VerifiablePresentation
        {
            Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
            Type = new List<string> { "VerifiablePresentation" },
            Holder = "did:web:user.com",
            VerifiableCredential = null!
        };

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithEmptyVerifiableCredential_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new VerifiablePresentation
        {
            Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
            Type = new List<string> { "VerifiablePresentation" },
            Holder = "did:web:user.com",
            VerifiableCredential = new List<VerifiableCredential>()
        };

        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region W3C VC 1.1 Compliance Tests

    [Fact]
    public void Constructor_WithW3CCompliantPresentation_ShouldCreateVerifiablePresentation()
    {
        // Arrange - Verifiable Presentation compliant with W3C VC 1.1 spec
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiablePresentation" };
        var holder = "did:web:user.com";
        var verifiableCredential = new VerifiableCredential
        {
            Context = new List<string> { "https://www.w3.org/2018/credentials/v1" },
            Type = new List<string> { "VerifiableCredential", "EmailCredential" },
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
        var proof = new Proof
        {
            Type = "Ed25519Signature2020",
            Created = DateTime.UtcNow,
            VerificationMethod = "did:web:user.com#key-1",
            ProofPurpose = "authentication",
            ProofValue = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
        };

        // Act
        var verifiablePresentation = new VerifiablePresentation
        {
            Context = context,
            Type = type,
            Holder = holder,
            VerifiableCredential = new List<VerifiableCredential> { verifiableCredential },
            Proof = proof
        };

        // Assert
        verifiablePresentation.Context.Should().Contain("https://www.w3.org/2018/credentials/v1");
        verifiablePresentation.Type.Should().Contain("VerifiablePresentation");
        verifiablePresentation.Holder.Should().Be(holder);
        verifiablePresentation.VerifiableCredential.Should().Contain(verifiableCredential);
        verifiablePresentation.Proof.Should().Be(proof);
    }

    [Fact]
    public void Constructor_WithAuthenticationProof_ShouldCreateVerifiablePresentation()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiablePresentation" };
        var holder = "did:web:user.com";
        var verifiableCredential = new VerifiableCredential
        {
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
        var proof = new Proof
        {
            Type = "Ed25519Signature2020",
            Created = DateTime.UtcNow,
            VerificationMethod = "did:web:user.com#key-1",
            ProofPurpose = "authentication",
            ProofValue = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
        };

        // Act
        var verifiablePresentation = new VerifiablePresentation
        {
            Context = context,
            Type = type,
            Holder = holder,
            VerifiableCredential = new List<VerifiableCredential> { verifiableCredential },
            Proof = proof
        };

        // Assert
        verifiablePresentation.Proof.Type.Should().Be("Ed25519Signature2020");
        verifiablePresentation.Proof.ProofPurpose.Should().Be("authentication");
    }

    [Fact]
    public void Constructor_WithAssertionMethodProof_ShouldCreateVerifiablePresentation()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiablePresentation" };
        var holder = "did:web:user.com";
        var verifiableCredential = new VerifiableCredential
        {
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
        var proof = new Proof
        {
            Type = "Ed25519Signature2020",
            Created = DateTime.UtcNow,
            VerificationMethod = "did:web:user.com#key-1",
            ProofPurpose = "assertionMethod",
            ProofValue = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
        };

        // Act
        var verifiablePresentation = new VerifiablePresentation
        {
            Context = context,
            Type = type,
            Holder = holder,
            VerifiableCredential = new List<VerifiableCredential> { verifiableCredential },
            Proof = proof
        };

        // Assert
        verifiablePresentation.Proof.Type.Should().Be("Ed25519Signature2020");
        verifiablePresentation.Proof.ProofPurpose.Should().Be("assertionMethod");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Constructor_WithEmptyVerifiableCredentialList_ShouldCreateVerifiablePresentation()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiablePresentation" };
        var holder = "did:web:user.com";

        // Act
        var verifiablePresentation = new VerifiablePresentation
        {
            Context = context,
            Type = type,
            Holder = holder,
            VerifiableCredential = new List<VerifiableCredential>()
        };

        // Assert
        verifiablePresentation.VerifiableCredential.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithNullOptionalFields_ShouldCreateVerifiablePresentation()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiablePresentation" };
        var holder = "did:web:user.com";
        var verifiableCredential = new VerifiableCredential
        {
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

        // Act
        var verifiablePresentation = new VerifiablePresentation
        {
            Id = null,
            Context = context,
            Type = type,
            Holder = holder,
            VerifiableCredential = new List<VerifiableCredential> { verifiableCredential },
            Proof = null
        };

        // Assert
        verifiablePresentation.Id.Should().BeNull();
        verifiablePresentation.Proof.Should().BeNull();
    }

    [Fact]
    public void Equality_WithSamePresentation_ShouldBeEqual()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiablePresentation" };
        var holder = "did:web:user.com";
        var verifiableCredential = new VerifiableCredential
        {
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

        var presentation1 = new VerifiablePresentation
        {
            Context = context,
            Type = type,
            Holder = holder,
            VerifiableCredential = new List<VerifiableCredential> { verifiableCredential }
        };

        var presentation2 = new VerifiablePresentation
        {
            Context = context,
            Type = type,
            Holder = holder,
            VerifiableCredential = new List<VerifiableCredential> { verifiableCredential }
        };

        // Act & Assert
        presentation1.Should().Be(presentation2);
        presentation1.GetHashCode().Should().Be(presentation2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentPresentation_ShouldNotBeEqual()
    {
        // Arrange
        var context = new List<string> { "https://www.w3.org/2018/credentials/v1" };
        var type = new List<string> { "VerifiablePresentation" };
        var holder1 = "did:web:user1.com";
        var holder2 = "did:web:user2.com";
        var verifiableCredential = new VerifiableCredential
        {
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

        var presentation1 = new VerifiablePresentation
        {
            Context = context,
            Type = type,
            Holder = holder1,
            VerifiableCredential = new List<VerifiableCredential> { verifiableCredential }
        };

        var presentation2 = new VerifiablePresentation
        {
            Context = context,
            Type = type,
            Holder = holder2,
            VerifiableCredential = new List<VerifiableCredential> { verifiableCredential }
        };

        // Act & Assert
        presentation1.Should().NotBe(presentation2);
    }

    #endregion
}
