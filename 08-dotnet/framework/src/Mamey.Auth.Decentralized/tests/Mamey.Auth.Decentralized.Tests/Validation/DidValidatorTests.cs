using FluentAssertions;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Exceptions;
using Mamey.Auth.Decentralized.Validation;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.Validation;

/// <summary>
/// Tests for the DidValidator class covering W3C DID 1.1 compliance.
/// </summary>
public class DidValidatorTests
{
    #region Happy Path Tests

    [Theory]
    [InlineData("did:web:example.com")]
    [InlineData("did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw")]
    [InlineData("did:web:example.com:user:alice")]
    [InlineData("did:web:example.com:user:alice:profile")]
    public void IsValidDid_WithValidDid_ShouldReturnTrue(string did)
    {
        // Act
        var result = DidValidator.IsValidDid(did);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("did:web:example.com/path")]
    [InlineData("did:web:example.com?param=value")]
    [InlineData("did:web:example.com#fragment")]
    [InlineData("did:web:example.com/path?param=value#fragment")]
    public void IsValidDidUrl_WithValidDidUrl_ShouldReturnTrue(string didUrl)
    {
        // Act
        var result = DidValidator.IsValidDidUrl(didUrl);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("did:web:example.com")]
    [InlineData("did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw")]
    public void ParseDid_WithValidDid_ShouldReturnDid(string did)
    {
        // Act
        var result = DidValidator.ParseDid(did);

        // Assert
        result.Should().NotBeNull();
        result.ToString().Should().Be(did);
    }

    [Theory]
    [InlineData("did:web:example.com/path")]
    [InlineData("did:web:example.com?param=value")]
    [InlineData("did:web:example.com#fragment")]
    public void ParseDidUrl_WithValidDidUrl_ShouldReturnDidUrl(string didUrl)
    {
        // Act
        var result = DidValidator.ParseDidUrl(didUrl);

        // Assert
        result.Should().NotBeNull();
        result.ToString().Should().Be(didUrl);
    }

    [Theory]
    [InlineData("web", new[] { "web", "key" })]
    [InlineData("key", new[] { "web", "key" })]
    [InlineData("WEB", new[] { "web", "key" })]
    [InlineData("KEY", new[] { "web", "key" })]
    public void IsSupportedMethod_WithSupportedMethod_ShouldReturnTrue(string method, string[] supportedMethods)
    {
        // Act
        var result = DidValidator.IsSupportedMethod(method, supportedMethods);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("example.com")]
    [InlineData("user.alice")]
    [InlineData("user-alice")]
    [InlineData("user_alice")]
    [InlineData("user123")]
    public void IsValidIdentifier_WithValidIdentifier_ShouldReturnTrue(string identifier)
    {
        // Act
        var result = DidValidator.IsValidIdentifier(identifier);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("key-1")]
    [InlineData("key_1")]
    [InlineData("key.1")]
    [InlineData("key:1")]
    [InlineData("key/1")]
    public void IsValidFragment_WithValidFragment_ShouldReturnTrue(string fragment)
    {
        // Act
        var result = DidValidator.IsValidFragment(fragment);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("param=value")]
    [InlineData("param1=value1&param2=value2")]
    [InlineData("param=value&other=test")]
    public void IsValidQuery_WithValidQuery_ShouldReturnTrue(string query)
    {
        // Act
        var result = DidValidator.IsValidQuery(query);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Unhappy Path Tests

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("not-a-did")]
    [InlineData("did:")]
    [InlineData("did::")]
    [InlineData("did:web:")]
    public void IsValidDid_WithInvalidDid_ShouldReturnFalse(string did)
    {
        // Act
        var result = DidValidator.IsValidDid(did);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("not-a-did-url")]
    [InlineData("did:")]
    [InlineData("did::")]
    [InlineData("did:web:")]
    public void IsValidDidUrl_WithInvalidDidUrl_ShouldReturnFalse(string didUrl)
    {
        // Act
        var result = DidValidator.IsValidDidUrl(didUrl);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("not-a-did")]
    public void ParseDid_WithInvalidDid_ShouldThrowInvalidDidException(string did)
    {
        // Act & Assert
        var action = () => DidValidator.ParseDid(did);
        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("not-a-did-url")]
    public void ParseDidUrl_WithInvalidDidUrl_ShouldThrowInvalidDidException(string didUrl)
    {
        // Act & Assert
        var action = () => DidValidator.ParseDidUrl(didUrl);
        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("unsupported", new[] { "web", "key" })]
    [InlineData("", new[] { "web", "key" })]
    [InlineData("web", new string[0])]
    [InlineData("web", null)]
    public void IsSupportedMethod_WithUnsupportedMethod_ShouldReturnFalse(string method, string[] supportedMethods)
    {
        // Act
        var result = DidValidator.IsSupportedMethod(method, supportedMethods);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid identifier")]
    [InlineData("invalid@identifier")]
    [InlineData("invalid#identifier")]
    [InlineData("invalid?identifier")]
    public void IsValidIdentifier_WithInvalidIdentifier_ShouldReturnFalse(string identifier)
    {
        // Act
        var result = DidValidator.IsValidIdentifier(identifier);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("invalid fragment")]
    [InlineData("invalid@fragment")]
    [InlineData("invalid#fragment")]
    [InlineData("invalid?fragment")]
    public void IsValidFragment_WithInvalidFragment_ShouldReturnFalse(string fragment)
    {
        // Act
        var result = DidValidator.IsValidFragment(fragment);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("invalid query")]
    [InlineData("param=value&")]
    [InlineData("=value")]
    [InlineData("param=")]
    public void IsValidQuery_WithInvalidQuery_ShouldReturnFalse(string query)
    {
        // Act
        var result = DidValidator.IsValidQuery(query);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("unsupported", new[] { "web", "key" })]
    [InlineData("", new[] { "web", "key" })]
    [InlineData("web", new string[0])]
    [InlineData("web", null)]
    public void ValidateMethod_WithUnsupportedMethod_ShouldThrowUnsupportedDidMethodException(string method, string[] supportedMethods)
    {
        // Act & Assert
        var action = () => DidValidator.ValidateMethod(method, supportedMethods);
        action.Should().Throw<UnsupportedDidMethodException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid identifier")]
    public void ValidateIdentifier_WithInvalidIdentifier_ShouldThrowInvalidDidException(string identifier)
    {
        // Act & Assert
        var action = () => DidValidator.ValidateIdentifier(identifier);
        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("invalid fragment")]
    [InlineData("invalid@fragment")]
    public void ValidateFragment_WithInvalidFragment_ShouldThrowInvalidDidException(string fragment)
    {
        // Act & Assert
        var action = () => DidValidator.ValidateFragment(fragment);
        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("invalid query")]
    [InlineData("param=value&")]
    public void ValidateQuery_WithInvalidQuery_ShouldThrowInvalidDidException(string query)
    {
        // Act & Assert
        var action = () => DidValidator.ValidateQuery(query);
        action.Should().Throw<InvalidDidException>();
    }

    #endregion

    #region W3C DID 1.1 Compliance Tests

    [Fact]
    public void IsValidDid_WithW3CCompliantDid_ShouldReturnTrue()
    {
        // Arrange - DID compliant with W3C DID 1.1 spec
        var did = "did:web:example.com";

        // Act
        var result = DidValidator.IsValidDid(did);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidDidUrl_WithW3CCompliantDidUrl_ShouldReturnTrue()
    {
        // Arrange - DID URL compliant with W3C DID 1.1 spec
        var didUrl = "did:web:example.com/path?param=value#fragment";

        // Act
        var result = DidValidator.IsValidDidUrl(didUrl);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ParseDid_WithW3CCompliantDid_ShouldReturnDid()
    {
        // Arrange
        var did = "did:web:example.com";

        // Act
        var result = DidValidator.ParseDid(did);

        // Assert
        result.Should().NotBeNull();
        result.Method.Should().Be("web");
        result.Identifier.Should().Be("example.com");
    }

    [Fact]
    public void ParseDidUrl_WithW3CCompliantDidUrl_ShouldReturnDidUrl()
    {
        // Arrange
        var didUrl = "did:web:example.com/path?param=value#fragment";

        // Act
        var result = DidValidator.ParseDidUrl(didUrl);

        // Assert
        result.Should().NotBeNull();
        result.Did.Method.Should().Be("web");
        result.Did.Identifier.Should().Be("example.com");
        result.Path.Should().Be("/path");
        result.Query.Should().Be("param=value");
        result.Fragment.Should().Be("fragment");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void IsValidDid_WithEmptyString_ShouldReturnFalse()
    {
        // Act
        var result = DidValidator.IsValidDid("");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidDid_WithWhitespace_ShouldReturnFalse()
    {
        // Act
        var result = DidValidator.IsValidDid("   ");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidDid_WithNull_ShouldReturnFalse()
    {
        // Act
        var result = DidValidator.IsValidDid(null!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidDidUrl_WithEmptyString_ShouldReturnFalse()
    {
        // Act
        var result = DidValidator.IsValidDidUrl("");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidDidUrl_WithWhitespace_ShouldReturnFalse()
    {
        // Act
        var result = DidValidator.IsValidDidUrl("   ");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidDidUrl_WithNull_ShouldReturnFalse()
    {
        // Act
        var result = DidValidator.IsValidDidUrl(null!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsValidFragment_WithEmptyString_ShouldReturnTrue()
    {
        // Act
        var result = DidValidator.IsValidFragment("");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidFragment_WithNull_ShouldReturnTrue()
    {
        // Act
        var result = DidValidator.IsValidFragment(null!);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidQuery_WithEmptyString_ShouldReturnTrue()
    {
        // Act
        var result = DidValidator.IsValidQuery("");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsValidQuery_WithNull_ShouldReturnTrue()
    {
        // Act
        var result = DidValidator.IsValidQuery(null!);

        // Assert
        result.Should().BeTrue();
    }

    #endregion
}
