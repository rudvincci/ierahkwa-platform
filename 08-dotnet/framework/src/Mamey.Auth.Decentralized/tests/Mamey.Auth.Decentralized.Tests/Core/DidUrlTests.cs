using FluentAssertions;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Exceptions;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.Core;

/// <summary>
/// Tests for the DidUrl class covering W3C DID 1.1 compliance.
/// </summary>
public class DidUrlTests
{
    #region Happy Path Tests

    [Theory]
    [InlineData("did:web:example.com/path", "did:web:example.com", "/path", null, null)]
    [InlineData("did:web:example.com:user:alice/path", "did:web:example.com:user:alice", "/path", null, null)]
    [InlineData("did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw/path", "did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw", "/path", null, null)]
    public void Parse_ValidDidUrlWithPath_ShouldReturnCorrectComponents(
        string didUrlString, 
        string expectedDid, 
        string expectedPath, 
        string? expectedQuery, 
        string? expectedFragment)
    {
        // Act
        var didUrl = DidUrl.Parse(didUrlString);

        // Assert
        didUrl.ToString().Should().Be(didUrlString);
        didUrl.Did.ToString().Should().Be(expectedDid);
        didUrl.Path.Should().Be(expectedPath);
        didUrl.Query.Should().Be(expectedQuery);
        didUrl.Fragment.Should().Be(expectedFragment);
    }

    [Theory]
    [InlineData("did:web:example.com?param=value", "did:web:example.com", null, "param=value", null)]
    [InlineData("did:web:example.com?param1=value1&param2=value2", "did:web:example.com", null, "param1=value1&param2=value2", null)]
    public void Parse_ValidDidUrlWithQuery_ShouldReturnCorrectComponents(
        string didUrlString, 
        string expectedDid, 
        string? expectedPath, 
        string? expectedQuery, 
        string? expectedFragment)
    {
        // Act
        var didUrl = DidUrl.Parse(didUrlString);

        // Assert
        didUrl.ToString().Should().Be(didUrlString);
        didUrl.Did.ToString().Should().Be(expectedDid);
        didUrl.Path.Should().Be(expectedPath);
        didUrl.Query.Should().Be(expectedQuery);
        didUrl.Fragment.Should().Be(expectedFragment);
    }

    [Theory]
    [InlineData("did:web:example.com#fragment", "did:web:example.com", null, null, "fragment")]
    [InlineData("did:web:example.com#key-1", "did:web:example.com", null, null, "key-1")]
    public void Parse_ValidDidUrlWithFragment_ShouldReturnCorrectComponents(
        string didUrlString, 
        string expectedDid, 
        string? expectedPath, 
        string? expectedQuery, 
        string? expectedFragment)
    {
        // Act
        var didUrl = DidUrl.Parse(didUrlString);

        // Assert
        didUrl.ToString().Should().Be(didUrlString);
        didUrl.Did.ToString().Should().Be(expectedDid);
        didUrl.Path.Should().Be(expectedPath);
        didUrl.Query.Should().Be(expectedQuery);
        didUrl.Fragment.Should().Be(expectedFragment);
    }

    [Theory]
    [InlineData("did:web:example.com/path?param=value#fragment", "did:web:example.com", "/path", "param=value", "fragment")]
    [InlineData("did:web:example.com:user:alice/path?param=value#fragment", "did:web:example.com:user:alice", "/path", "param=value", "fragment")]
    public void Parse_ValidDidUrlWithAllComponents_ShouldReturnCorrectComponents(
        string didUrlString, 
        string expectedDid, 
        string? expectedPath, 
        string? expectedQuery, 
        string? expectedFragment)
    {
        // Act
        var didUrl = DidUrl.Parse(didUrlString);

        // Assert
        didUrl.ToString().Should().Be(didUrlString);
        didUrl.Did.ToString().Should().Be(expectedDid);
        didUrl.Path.Should().Be(expectedPath);
        didUrl.Query.Should().Be(expectedQuery);
        didUrl.Fragment.Should().Be(expectedFragment);
    }

    [Theory]
    [InlineData("did:web:example.com", "did:web:example.com", null, null, null)]
    [InlineData("did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw", "did:key:z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw", null, null, null)]
    public void Parse_ValidDidUrlWithoutComponents_ShouldReturnCorrectComponents(
        string didUrlString, 
        string expectedDid, 
        string? expectedPath, 
        string? expectedQuery, 
        string? expectedFragment)
    {
        // Act
        var didUrl = DidUrl.Parse(didUrlString);

        // Assert
        didUrl.ToString().Should().Be(didUrlString);
        didUrl.Did.ToString().Should().Be(expectedDid);
        didUrl.Path.Should().Be(expectedPath);
        didUrl.Query.Should().Be(expectedQuery);
        didUrl.Fragment.Should().Be(expectedFragment);
    }

    [Fact]
    public void Parse_RelativeDidUrl_ShouldHandleCorrectly()
    {
        // Arrange
        var relativeDidUrl = "#key-1";

        // Act
        var didUrl = DidUrl.Parse(relativeDidUrl);

        // Assert
        didUrl.Fragment.Should().Be("key-1");
        didUrl.Did.Should().BeNull();
    }

    [Fact]
    public void Dereference_ShouldReturnCorrectDid()
    {
        // Arrange
        var didUrlString = "did:web:example.com/path?param=value#fragment";
        var didUrl = DidUrl.Parse(didUrlString);

        // Act
        var dereferencedDid = didUrl.Dereference();

        // Assert
        dereferencedDid.Should().NotBeNull();
        dereferencedDid!.ToString().Should().Be("did:web:example.com");
    }

    #endregion

    #region Unhappy Path Tests

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid")]
    [InlineData("not-a-did-url")]
    [InlineData("did:")]
    [InlineData("did::")]
    [InlineData("did:web:")]
    public void Parse_InvalidDidUrlFormat_ShouldThrowInvalidDidException(string invalidDidUrl)
    {
        // Act & Assert
        var action = () => DidUrl.Parse(invalidDidUrl);
        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("did:web:example.com?invalid-query")]
    [InlineData("did:web:example.com?param=value&")]
    [InlineData("did:web:example.com?=value")]
    public void Parse_InvalidQueryFormat_ShouldThrowInvalidDidException(string didUrlWithInvalidQuery)
    {
        // Act & Assert
        var action = () => DidUrl.Parse(didUrlWithInvalidQuery);
        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("did:web:example.com#")]
    [InlineData("did:web:example.com#invalid fragment")]
    [InlineData("did:web:example.com#fragment with spaces")]
    public void Parse_InvalidFragmentFormat_ShouldThrowInvalidDidException(string didUrlWithInvalidFragment)
    {
        // Act & Assert
        var action = () => DidUrl.Parse(didUrlWithInvalidFragment);
        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("did:web:example.com//double-slash")]
    [InlineData("did:web:example.com/../invalid-path")]
    [InlineData("did:web:example.com/./invalid-path")]
    public void Parse_InvalidPathFormat_ShouldThrowInvalidDidException(string didUrlWithInvalidPath)
    {
        // Act & Assert
        var action = () => DidUrl.Parse(didUrlWithInvalidPath);
        action.Should().Throw<InvalidDidException>();
    }

    [Theory]
    [InlineData("did:web:example.com?param=value&param=value")]
    [InlineData("did:web:example.com?param1=value1&param1=value2")]
    public void Parse_DuplicateQueryParameters_ShouldHandleCorrectly(string didUrlWithDuplicateParams)
    {
        // Act
        var didUrl = DidUrl.Parse(didUrlWithDuplicateParams);

        // Assert
        didUrl.Query.Should().Be(didUrlWithDuplicateParams.Split('?')[1]);
    }

    #endregion

    #region W3C DID 1.1 Compliance Tests

    [Fact]
    public void Parse_ValidDidUrl_ShouldComplyWithW3CSpec()
    {
        // Arrange - Valid DID URL according to W3C spec
        var validDidUrl = "did:web:example.com/path?param=value#fragment";

        // Act
        var didUrl = DidUrl.Parse(validDidUrl);

        // Assert
        didUrl.Did.Method.Should().Be("web");
        didUrl.Did.Identifier.Should().Be("example.com");
        didUrl.Path.Should().Be("/path");
        didUrl.Query.Should().Be("param=value");
        didUrl.Fragment.Should().Be("fragment");
    }

    [Fact]
    public void Parse_RelativeDidUrl_ShouldComplyWithW3CSpec()
    {
        // Arrange - Relative DID URL according to W3C spec
        var relativeDidUrl = "#key-1";

        // Act
        var didUrl = DidUrl.Parse(relativeDidUrl);

        // Assert
        didUrl.Fragment.Should().Be("key-1");
        didUrl.Did.Should().BeNull();
    }

    [Fact]
    public void Parse_DidUrlWithComplexPath_ShouldComplyWithW3CSpec()
    {
        // Arrange
        var complexDidUrl = "did:web:example.com:user:alice/path/to/resource";

        // Act
        var didUrl = DidUrl.Parse(complexDidUrl);

        // Assert
        didUrl.Did.Method.Should().Be("web");
        didUrl.Did.Identifier.Should().Be("example.com:user:alice");
        didUrl.Path.Should().Be("/path/to/resource");
    }

    [Fact]
    public void Parse_DidUrlWithMultipleQueryParams_ShouldComplyWithW3CSpec()
    {
        // Arrange
        var didUrlWithMultipleParams = "did:web:example.com?param1=value1&param2=value2&param3=value3";

        // Act
        var didUrl = DidUrl.Parse(didUrlWithMultipleParams);

        // Assert
        didUrl.Query.Should().Be("param1=value1&param2=value2&param3=value3");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Parse_DidUrlWithSpecialCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var didUrlWithSpecialChars = "did:web:example.com/path-with-hyphens_and_underscores?param=value%20with%20spaces#fragment-with-special-chars";

        // Act
        var didUrl = DidUrl.Parse(didUrlWithSpecialChars);

        // Assert
        didUrl.Path.Should().Be("/path-with-hyphens_and_underscores");
        didUrl.Query.Should().Be("param=value%20with%20spaces");
        didUrl.Fragment.Should().Be("fragment-with-special-chars");
    }

    [Fact]
    public void Parse_DidUrlWithEmptyComponents_ShouldHandleCorrectly()
    {
        // Arrange
        var didUrlWithEmptyComponents = "did:web:example.com/?param=#fragment";

        // Act
        var didUrl = DidUrl.Parse(didUrlWithEmptyComponents);

        // Assert
        didUrl.Path.Should().Be("/");
        didUrl.Query.Should().Be("param=");
        didUrl.Fragment.Should().Be("fragment");
    }

    [Fact]
    public void Parse_DidUrlWithEncodedCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var didUrlWithEncodedChars = "did:web:example.com/path%20with%20spaces?param=value%26encoded#fragment%20with%20spaces";

        // Act
        var didUrl = DidUrl.Parse(didUrlWithEncodedChars);

        // Assert
        didUrl.Path.Should().Be("/path%20with%20spaces");
        didUrl.Query.Should().Be("param=value%26encoded");
        didUrl.Fragment.Should().Be("fragment%20with%20spaces");
    }

    [Fact]
    public void ToString_ShouldReconstructOriginalDidUrl()
    {
        // Arrange
        var originalDidUrl = "did:web:example.com/path?param=value#fragment";
        var didUrl = DidUrl.Parse(originalDidUrl);

        // Act
        var reconstructed = didUrl.ToString();

        // Assert
        reconstructed.Should().Be(originalDidUrl);
    }

    [Fact]
    public void Equality_WithSameDidUrl_ShouldBeEqual()
    {
        // Arrange
        var didUrl1 = DidUrl.Parse("did:web:example.com/path?param=value#fragment");
        var didUrl2 = DidUrl.Parse("did:web:example.com/path?param=value#fragment");

        // Act & Assert
        didUrl1.Should().Be(didUrl2);
        didUrl1.GetHashCode().Should().Be(didUrl2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentDidUrl_ShouldNotBeEqual()
    {
        // Arrange
        var didUrl1 = DidUrl.Parse("did:web:example.com/path?param=value#fragment");
        var didUrl2 = DidUrl.Parse("did:web:example.com/path?param=value#different");

        // Act & Assert
        didUrl1.Should().NotBe(didUrl2);
    }

    #endregion
}
