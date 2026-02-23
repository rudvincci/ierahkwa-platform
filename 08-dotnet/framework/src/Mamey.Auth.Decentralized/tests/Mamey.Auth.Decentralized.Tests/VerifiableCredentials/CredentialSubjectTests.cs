using FluentAssertions;
using Mamey.Auth.Decentralized.VerifiableCredentials;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.VerifiableCredentials;

/// <summary>
/// Tests for the CredentialSubject class covering W3C VC 1.1 compliance.
/// </summary>
public class CredentialSubjectTests
{
    #region Happy Path Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateCredentialSubject()
    {
        // Arrange
        var id = "did:web:user.com";
        var properties = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["name"] = "John Doe",
            ["age"] = 30,
            ["verified"] = true
        };

        // Act
        var credentialSubject = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        // Assert
        credentialSubject.Id.Should().Be(id);
        credentialSubject.Properties.Should().BeEquivalentTo(properties);
    }

    [Fact]
    public void Constructor_WithMinimalRequiredFields_ShouldCreateCredentialSubject()
    {
        // Arrange
        var id = "did:web:user.com";
        var properties = new Dictionary<string, object>
        {
            ["email"] = "user@example.com"
        };

        // Act
        var credentialSubject = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        // Assert
        credentialSubject.Id.Should().Be(id);
        credentialSubject.Properties.Should().BeEquivalentTo(properties);
    }

    [Fact]
    public void Constructor_WithEmptyProperties_ShouldCreateCredentialSubject()
    {
        // Arrange
        var id = "did:web:user.com";
        var properties = new Dictionary<string, object>();

        // Act
        var credentialSubject = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        // Assert
        credentialSubject.Id.Should().Be(id);
        credentialSubject.Properties.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_WithStringProperties_ShouldCreateCredentialSubject()
    {
        // Arrange
        var id = "did:web:user.com";
        var properties = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["name"] = "John Doe",
            ["description"] = "A verified user"
        };

        // Act
        var credentialSubject = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        // Assert
        credentialSubject.Properties.Should().ContainKey("email");
        credentialSubject.Properties.Should().ContainKey("name");
        credentialSubject.Properties.Should().ContainKey("description");
        credentialSubject.Properties["email"].Should().Be("user@example.com");
        credentialSubject.Properties["name"].Should().Be("John Doe");
        credentialSubject.Properties["description"].Should().Be("A verified user");
    }

    [Fact]
    public void Constructor_WithNumericProperties_ShouldCreateCredentialSubject()
    {
        // Arrange
        var id = "did:web:user.com";
        var properties = new Dictionary<string, object>
        {
            ["age"] = 30,
            ["score"] = 95.5,
            ["count"] = 100
        };

        // Act
        var credentialSubject = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        // Assert
        credentialSubject.Properties.Should().ContainKey("age");
        credentialSubject.Properties.Should().ContainKey("score");
        credentialSubject.Properties.Should().ContainKey("count");
        credentialSubject.Properties["age"].Should().Be(30);
        credentialSubject.Properties["score"].Should().Be(95.5);
        credentialSubject.Properties["count"].Should().Be(100);
    }

    [Fact]
    public void Constructor_WithBooleanProperties_ShouldCreateCredentialSubject()
    {
        // Arrange
        var id = "did:web:user.com";
        var properties = new Dictionary<string, object>
        {
            ["verified"] = true,
            ["active"] = false,
            ["enabled"] = true
        };

        // Act
        var credentialSubject = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        // Assert
        credentialSubject.Properties.Should().ContainKey("verified");
        credentialSubject.Properties.Should().ContainKey("active");
        credentialSubject.Properties.Should().ContainKey("enabled");
        credentialSubject.Properties["verified"].Should().Be(true);
        credentialSubject.Properties["active"].Should().Be(false);
        credentialSubject.Properties["enabled"].Should().Be(true);
    }

    [Fact]
    public void Constructor_WithArrayProperties_ShouldCreateCredentialSubject()
    {
        // Arrange
        var id = "did:web:user.com";
        var properties = new Dictionary<string, object>
        {
            ["roles"] = new[] { "user", "admin", "moderator" },
            ["permissions"] = new[] { "read", "write", "delete" },
            ["tags"] = new[] { "verified", "premium", "active" }
        };

        // Act
        var credentialSubject = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        // Assert
        credentialSubject.Properties.Should().ContainKey("roles");
        credentialSubject.Properties.Should().ContainKey("permissions");
        credentialSubject.Properties.Should().ContainKey("tags");
        credentialSubject.Properties["roles"].Should().BeEquivalentTo(new[] { "user", "admin", "moderator" });
        credentialSubject.Properties["permissions"].Should().BeEquivalentTo(new[] { "read", "write", "delete" });
        credentialSubject.Properties["tags"].Should().BeEquivalentTo(new[] { "verified", "premium", "active" });
    }

    [Fact]
    public void Constructor_WithObjectProperties_ShouldCreateCredentialSubject()
    {
        // Arrange
        var id = "did:web:user.com";
        var properties = new Dictionary<string, object>
        {
            ["address"] = new
            {
                street = "123 Main St",
                city = "Anytown",
                state = "CA",
                zip = "12345"
            },
            ["preferences"] = new
            {
                theme = "dark",
                language = "en",
                notifications = true
            }
        };

        // Act
        var credentialSubject = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        // Assert
        credentialSubject.Properties.Should().ContainKey("address");
        credentialSubject.Properties.Should().ContainKey("preferences");
        credentialSubject.Properties["address"].Should().NotBeNull();
        credentialSubject.Properties["preferences"].Should().NotBeNull();
    }

    #endregion

    #region Unhappy Path Tests

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-id")]
    [InlineData("not-a-did")]
    public void Constructor_WithInvalidId_ShouldThrowArgumentException(string invalidId)
    {
        // Act & Assert
        var action = () => new CredentialSubject
        {
            Id = invalidId,
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user@example.com"
            }
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithNullProperties_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new CredentialSubject
        {
            Id = "did:web:user.com",
            Properties = null!
        };

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithEmptyId_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new CredentialSubject
        {
            Id = "",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user@example.com"
            }
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithWhitespaceId_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new CredentialSubject
        {
            Id = "   ",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user@example.com"
            }
        };

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithInvalidDidFormat_ShouldThrowArgumentException()
    {
        // Act & Assert
        var action = () => new CredentialSubject
        {
            Id = "not-a-did",
            Properties = new Dictionary<string, object>
            {
                ["email"] = "user@example.com"
            }
        };

        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region W3C VC 1.1 Compliance Tests

    [Fact]
    public void Constructor_WithW3CCompliantCredentialSubject_ShouldCreateCredentialSubject()
    {
        // Arrange - Credential Subject compliant with W3C VC 1.1 spec
        var id = "did:web:user.com";
        var properties = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["name"] = "John Doe",
            ["birthDate"] = "1990-01-01",
            ["nationality"] = "US",
            ["address"] = new
            {
                street = "123 Main St",
                city = "Anytown",
                state = "CA",
                zip = "12345",
                country = "US"
            }
        };

        // Act
        var credentialSubject = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        // Assert
        credentialSubject.Id.Should().Be(id);
        credentialSubject.Properties.Should().BeEquivalentTo(properties);
    }

    [Fact]
    public void Constructor_WithIdentityCredentialSubject_ShouldCreateCredentialSubject()
    {
        // Arrange
        var id = "did:web:user.com";
        var properties = new Dictionary<string, object>
        {
            ["type"] = "Person",
            ["givenName"] = "John",
            ["familyName"] = "Doe",
            ["email"] = "user@example.com",
            ["phoneNumber"] = "+1-555-123-4567",
            ["birthDate"] = "1990-01-01",
            ["gender"] = "male",
            ["nationality"] = "US"
        };

        // Act
        var credentialSubject = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        // Assert
        credentialSubject.Properties.Should().ContainKey("type");
        credentialSubject.Properties.Should().ContainKey("givenName");
        credentialSubject.Properties.Should().ContainKey("familyName");
        credentialSubject.Properties["type"].Should().Be("Person");
        credentialSubject.Properties["givenName"].Should().Be("John");
        credentialSubject.Properties["familyName"].Should().Be("Doe");
    }

    [Fact]
    public void Constructor_WithEmailCredentialSubject_ShouldCreateCredentialSubject()
    {
        // Arrange
        var id = "did:web:user.com";
        var properties = new Dictionary<string, object>
        {
            ["type"] = "EmailCredential",
            ["email"] = "user@example.com",
            ["verified"] = true,
            ["verificationDate"] = "2023-01-01T00:00:00Z",
            ["verificationMethod"] = "email-verification"
        };

        // Act
        var credentialSubject = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        // Assert
        credentialSubject.Properties.Should().ContainKey("type");
        credentialSubject.Properties.Should().ContainKey("email");
        credentialSubject.Properties.Should().ContainKey("verified");
        credentialSubject.Properties["type"].Should().Be("EmailCredential");
        credentialSubject.Properties["email"].Should().Be("user@example.com");
        credentialSubject.Properties["verified"].Should().Be(true);
    }

    [Fact]
    public void Constructor_WithOrganizationCredentialSubject_ShouldCreateCredentialSubject()
    {
        // Arrange
        var id = "did:web:organization.com";
        var properties = new Dictionary<string, object>
        {
            ["type"] = "Organization",
            ["name"] = "Example Corp",
            ["legalName"] = "Example Corporation Inc.",
            ["taxId"] = "12-3456789",
            ["website"] = "https://example.com",
            ["address"] = new
            {
                street = "123 Business St",
                city = "Business City",
                state = "CA",
                zip = "12345",
                country = "US"
            }
        };

        // Act
        var credentialSubject = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        // Assert
        credentialSubject.Properties.Should().ContainKey("type");
        credentialSubject.Properties.Should().ContainKey("name");
        credentialSubject.Properties.Should().ContainKey("legalName");
        credentialSubject.Properties["type"].Should().Be("Organization");
        credentialSubject.Properties["name"].Should().Be("Example Corp");
        credentialSubject.Properties["legalName"].Should().Be("Example Corporation Inc.");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Constructor_WithNullId_ShouldCreateCredentialSubject()
    {
        // Arrange
        var properties = new Dictionary<string, object>
        {
            ["email"] = "user@example.com"
        };

        // Act
        var credentialSubject = new CredentialSubject
        {
            Id = null,
            Properties = properties
        };

        // Assert
        credentialSubject.Id.Should().BeNull();
        credentialSubject.Properties.Should().BeEquivalentTo(properties);
    }

    [Fact]
    public void Constructor_WithSpecialCharactersInProperties_ShouldCreateCredentialSubject()
    {
        // Arrange
        var id = "did:web:user.com";
        var properties = new Dictionary<string, object>
        {
            ["email"] = "user+test@example.com",
            ["name"] = "John O'Connor",
            ["description"] = "User with special chars: !@#$%^&*()",
            ["unicode"] = "Unicode: 你好世界 🌍"
        };

        // Act
        var credentialSubject = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        // Assert
        credentialSubject.Properties.Should().ContainKey("email");
        credentialSubject.Properties.Should().ContainKey("name");
        credentialSubject.Properties.Should().ContainKey("description");
        credentialSubject.Properties.Should().ContainKey("unicode");
        credentialSubject.Properties["email"].Should().Be("user+test@example.com");
        credentialSubject.Properties["name"].Should().Be("John O'Connor");
        credentialSubject.Properties["description"].Should().Be("User with special chars: !@#$%^&*()");
        credentialSubject.Properties["unicode"].Should().Be("Unicode: 你好世界 🌍");
    }

    [Fact]
    public void Constructor_WithLargeProperties_ShouldCreateCredentialSubject()
    {
        // Arrange
        var id = "did:web:user.com";
        var properties = new Dictionary<string, object>();
        
        // Add many properties
        for (int i = 0; i < 100; i++)
        {
            properties[$"property_{i}"] = $"value_{i}";
        }

        // Act
        var credentialSubject = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        // Assert
        credentialSubject.Properties.Should().HaveCount(100);
        credentialSubject.Properties.Should().ContainKey("property_0");
        credentialSubject.Properties.Should().ContainKey("property_99");
        credentialSubject.Properties["property_0"].Should().Be("value_0");
        credentialSubject.Properties["property_99"].Should().Be("value_99");
    }

    [Fact]
    public void Equality_WithSameCredentialSubject_ShouldBeEqual()
    {
        // Arrange
        var id = "did:web:user.com";
        var properties = new Dictionary<string, object>
        {
            ["email"] = "user@example.com",
            ["name"] = "John Doe"
        };

        var credentialSubject1 = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        var credentialSubject2 = new CredentialSubject
        {
            Id = id,
            Properties = properties
        };

        // Act & Assert
        credentialSubject1.Should().Be(credentialSubject2);
        credentialSubject1.GetHashCode().Should().Be(credentialSubject2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentCredentialSubject_ShouldNotBeEqual()
    {
        // Arrange
        var id = "did:web:user.com";
        var properties1 = new Dictionary<string, object>
        {
            ["email"] = "user1@example.com",
            ["name"] = "John Doe"
        };
        var properties2 = new Dictionary<string, object>
        {
            ["email"] = "user2@example.com",
            ["name"] = "Jane Doe"
        };

        var credentialSubject1 = new CredentialSubject
        {
            Id = id,
            Properties = properties1
        };

        var credentialSubject2 = new CredentialSubject
        {
            Id = id,
            Properties = properties2
        };

        // Act & Assert
        credentialSubject1.Should().NotBe(credentialSubject2);
    }

    #endregion
}
