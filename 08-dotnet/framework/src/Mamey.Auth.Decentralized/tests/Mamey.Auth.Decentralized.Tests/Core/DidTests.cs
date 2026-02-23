using FluentAssertions;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Exceptions;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.Core;

/// <summary>
/// Tests for the Did class covering W3C DID 1.1 compliance.
/// </summary>
public class DidTests
{
    #region Happy Path Tests

    [Theory]
    [InlineData("did:web:example.com")]
    [InlineData("did:web:example.com:user:alice")]
    [InlineData("did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw")]
    [InlineData("did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw#key-1")]
    public void Parse_ValidDid_ShouldReturnDidObject(string didString)
    {
        // Act
        var did = Did.Parse(didString);

        // Assert
        did.Should().NotBeNull();
        did.ToString().Should().Be(didString);
    }

    [Theory]
    [InlineData("web", "example.com", "did:web:example.com")]
    [InlineData("key", "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw", "did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw")]
    public void Create_ValidMethodAndIdentifier_ShouldCreateDid(string method, string identifier, string expectedDid)
    {
        // Act
        var did = Did.Parse(expectedDid);

        // Assert
        did.Method.Should().Be(method);
        did.Identifier.Should().Be(identifier);
        did.ToString().Should().Be(expectedDid);
    }

    [Fact]
    public void ToString_ShouldReturnCorrectDidString()
    {
        // Arrange
        var didString = "did:web:example.com";
        var did = Did.Parse(didString);

        // Act
        var result = did.ToString();

        // Assert
        result.Should().Be(didString);
    }

    [Fact]
    public void Equality_WithSameDid_ShouldBeEqual()
    {
        // Arrange
        var did1 = Did.Parse("did:web:example.com");
        var did2 = Did.Parse("did:web:example.com");

        // Act & Assert
        did1.Should().Be(did2);
        did1.GetHashCode().Should().Be(did2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentDid_ShouldNotBeEqual()
    {
        // Arrange
        var did1 = Did.Parse("did:web:example.com");
        var did2 = Did.Parse("did:web:example.org");

        // Act & Assert
        did1.Should().NotBe(did2);
    }

    [Fact]
    public void GetHashCode_ShouldBeConsistent()
    {
        // Arrange
        var did = Did.Parse("did:web:example.com");
        var hash1 = did.GetHashCode();
        var hash2 = did.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    #endregion

    #region Unhappy Path Tests

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("did:")]
    [InlineData("did::")]
    [InlineData("did:web:")]
    [InlineData("not-a-did")]
    [InlineData("did:web:example.com:")]
    public void Parse_InvalidDidFormat_ShouldThrowInvalidDidException(string invalidDid)
    {
        // Act & Assert
        var action = () => Did.Parse(invalidDid);
        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("did:unsupported:example.com")]
    [InlineData("did:unknown:test")]
    public void Parse_UnsupportedMethod_ShouldThrowUnsupportedDidMethodException(string didWithUnsupportedMethod)
    {
        // Act & Assert
        var action = () => Did.Parse(didWithUnsupportedMethod);
        action.Should().Throw<UnsupportedDidMethodException>();
    }

    [Theory]
    [InlineData("did:web:")]
    [InlineData("did:key:")]
    [InlineData("did:web:example.com:")]
    public void Parse_EmptyIdentifier_ShouldThrowInvalidDidException(string didWithEmptyIdentifier)
    {
        // Act & Assert
        var action = () => Did.Parse(didWithEmptyIdentifier);
        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("did:web:example.com!@#")]
    [InlineData("did:key:invalid-characters!@#")]
    public void Parse_InvalidIdentifierCharacters_ShouldThrowInvalidDidException(string didWithInvalidChars)
    {
        // Act & Assert
        var action = () => Did.Parse(didWithInvalidChars);
        action.Should().Throw<InvalidDidException>();
    }

    [Fact]
    public void Parse_DidExceedingLengthLimit_ShouldThrowInvalidDidException()
    {
        // Arrange
        var longIdentifier = new string('a', 3000); // Exceeds typical DID length limits
        var longDid = $"did:web:{longIdentifier}";

        // Act & Assert
        var action = () => Did.Parse(longDid);
        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("did:web:example..com")] // Double dots
    [InlineData("did:web:.example.com")] // Leading dot
    [InlineData("did:web:example.com.")] // Trailing dot
    public void Parse_InvalidMethodSpecificIdentifier_ShouldThrowInvalidDidException(string invalidMethodSpecificDid)
    {
        // Act & Assert
        var action = () => Did.Parse(invalidMethodSpecificDid);
        action.Should().Throw<InvalidDidException>();
    }

    #endregion

    #region W3C DID 1.1 Compliance Tests

    [Fact]
    public void Parse_ValidWebDid_ShouldComplyWithW3CSpec()
    {
        // Arrange - Valid did:web according to W3C spec
        var validWebDid = "did:web:example.com";

        // Act
        var did = Did.Parse(validWebDid);

        // Assert
        did.Method.Should().Be("web");
        did.Identifier.Should().Be("example.com");
        did.ToString().Should().Be(validWebDid);
    }

    [Fact]
    public void Parse_ValidKeyDid_ShouldComplyWithW3CSpec()
    {
        // Arrange - Valid did:key according to W3C spec
        var validKeyDid = "did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw";

        // Act
        var did = Did.Parse(validKeyDid);

        // Assert
        did.Method.Should().Be("key");
        did.Identifier.Should().Be("z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw");
        did.ToString().Should().Be(validKeyDid);
    }

    [Theory]
    [InlineData("did:web:example.com:user:alice")]
    [InlineData("did:web:example.com:path:to:resource")]
    public void Parse_WebDidWithPath_ShouldComplyWithW3CSpec(string webDidWithPath)
    {
        // Act
        var did = Did.Parse(webDidWithPath);

        // Assert
        did.Method.Should().Be("web");
        did.Identifier.Should().Contain("example.com");
        did.ToString().Should().Be(webDidWithPath);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Parse_DidWithSpecialCharacters_ShouldHandleCorrectly()
    {
        // Arrange - DIDs with special characters that are valid
        var didWithHyphens = "did:web:example-site.com";
        var didWithUnderscores = "did:web:example_site.com";

        // Act & Assert
        var did1 = Did.Parse(didWithHyphens);
        var did2 = Did.Parse(didWithUnderscores);

        did1.Identifier.Should().Be("example-site.com");
        did2.Identifier.Should().Be("example_site.com");
    }

    [Fact]
    public void Parse_DidWithNumbers_ShouldHandleCorrectly()
    {
        // Arrange
        var didWithNumbers = "did:web:example123.com";

        // Act
        var did = Did.Parse(didWithNumbers);

        // Assert
        did.Identifier.Should().Be("example123.com");
    }

    [Fact]
    public void Parse_DidWithMixedCase_ShouldPreserveCase()
    {
        // Arrange
        var didWithMixedCase = "did:web:Example.COM";

        // Act
        var did = Did.Parse(didWithMixedCase);

        // Assert
        did.Identifier.Should().Be("Example.COM");
    }

    #endregion
}
