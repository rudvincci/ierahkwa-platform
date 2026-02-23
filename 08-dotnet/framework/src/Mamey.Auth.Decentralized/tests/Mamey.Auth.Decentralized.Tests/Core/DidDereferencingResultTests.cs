using FluentAssertions;
using Mamey.Auth.Decentralized.Core;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.Core;

/// <summary>
/// Tests for the DidDereferencingResult class covering W3C DID 1.1 compliance.
/// </summary>
public class DidDereferencingResultTests
{
    #region Happy Path Tests

    [Fact]
    public void Success_WithValidContent_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var content = new { key = "value" };
        var dereferencingMetadata = new DidDereferencingMetadata
        {
            ContentType = "application/json",
            ResolutionTime = TimeSpan.FromMilliseconds(100)
        };

        // Act
        var result = DidDereferencingResult.Success(content, dereferencingMetadata);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Content.Should().Be(content);
        result.DereferencingMetadata.Should().Be(dereferencingMetadata);
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Success_WithMinimalParameters_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var content = new { key = "value" };

        // Act
        var result = DidDereferencingResult.Success(content);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Content.Should().Be(content);
        result.DereferencingMetadata.Should().NotBeNull();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failed_WithErrorMessage_ShouldCreateFailedResult()
    {
        // Arrange
        var errorMessage = "DID dereferencing failed";

        // Act
        var result = DidDereferencingResult.Failed(errorMessage);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Content.Should().BeNull();
        result.DereferencingMetadata.Should().NotBeNull();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public void InvalidDidUrl_WithErrorMessage_ShouldCreateInvalidDidUrlResult()
    {
        // Arrange
        var errorMessage = "Invalid DID URL format";

        // Act
        var result = DidDereferencingResult.InvalidDidUrl(errorMessage);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Content.Should().BeNull();
        result.DereferencingMetadata.Should().NotBeNull();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public void MethodNotSupported_WithErrorMessage_ShouldCreateMethodNotSupportedResult()
    {
        // Arrange
        var errorMessage = "DID method not supported";

        // Act
        var result = DidDereferencingResult.MethodNotSupported(errorMessage);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Content.Should().BeNull();
        result.DereferencingMetadata.Should().NotBeNull();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public void NotFound_WithErrorMessage_ShouldCreateNotFoundResult()
    {
        // Arrange
        var errorMessage = "DID URL not found";

        // Act
        var result = DidDereferencingResult.NotFound(errorMessage);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Content.Should().BeNull();
        result.DereferencingMetadata.Should().NotBeNull();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public void Deactivated_WithErrorMessage_ShouldCreateDeactivatedResult()
    {
        // Arrange
        var errorMessage = "DID has been deactivated";

        // Act
        var result = DidDereferencingResult.Deactivated(errorMessage);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Content.Should().BeNull();
        result.DereferencingMetadata.Should().NotBeNull();
        result.Error.Should().Be(errorMessage);
    }

    #endregion

    #region Unhappy Path Tests

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Failed_WithInvalidErrorMessage_ShouldThrowArgumentException(string invalidErrorMessage)
    {
        // Act & Assert
        var action = () => DidDereferencingResult.Failed(invalidErrorMessage);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void InvalidDidUrl_WithInvalidErrorMessage_ShouldThrowArgumentException(string invalidErrorMessage)
    {
        // Act & Assert
        var action = () => DidDereferencingResult.InvalidDidUrl(invalidErrorMessage);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void MethodNotSupported_WithInvalidErrorMessage_ShouldThrowArgumentException(string invalidErrorMessage)
    {
        // Act & Assert
        var action = () => DidDereferencingResult.MethodNotSupported(invalidErrorMessage);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void NotFound_WithInvalidErrorMessage_ShouldThrowArgumentException(string invalidErrorMessage)
    {
        // Act & Assert
        var action = () => DidDereferencingResult.NotFound(invalidErrorMessage);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Deactivated_WithInvalidErrorMessage_ShouldThrowArgumentException(string invalidErrorMessage)
    {
        // Act & Assert
        var action = () => DidDereferencingResult.Deactivated(invalidErrorMessage);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Success_WithNullContent_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => DidDereferencingResult.Success(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region W3C DID 1.1 Compliance Tests

    [Fact]
    public void Success_WithW3CCompliantContent_ShouldCreateSuccessfulResult()
    {
        // Arrange - Content compliant with W3C DID 1.1 spec
        var content = new
        {
            id = "did:web:example.com#key-1",
            type = "Ed25519VerificationKey2020",
            controller = "did:web:example.com",
            publicKeyJwk = new
            {
                kty = "OKP",
                crv = "Ed25519",
                x = "z6MkhaXgBZDvotDkL2577ULvitBVkCh6K4rDi2z7k5oDvsw"
            }
        };
        var dereferencingMetadata = new DidDereferencingMetadata
        {
            ContentType = "application/did+ld+json",
            ResolutionTime = TimeSpan.FromMilliseconds(100),
            Method = "web"
        };

        // Act
        var result = DidDereferencingResult.Success(content, dereferencingMetadata);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Content.Should().Be(content);
        result.DereferencingMetadata.Should().Be(dereferencingMetadata);
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Success_WithDereferencingMetadata_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var content = new { key = "value" };
        var dereferencingMetadata = new DidDereferencingMetadata
        {
            ContentType = "application/json",
            ResolutionTime = TimeSpan.FromMilliseconds(100),
            Method = "web",
            Driver = "web-resolver",
            DriverUrl = "https://resolver.example.com"
        };

        // Act
        var result = DidDereferencingResult.Success(content, dereferencingMetadata);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.DereferencingMetadata.Should().Be(dereferencingMetadata);
        result.DereferencingMetadata.ContentType.Should().Be("application/json");
        result.DereferencingMetadata.ResolutionTime.Should().Be(TimeSpan.FromMilliseconds(100));
        result.DereferencingMetadata.Method.Should().Be("web");
        result.DereferencingMetadata.Driver.Should().Be("web-resolver");
        result.DereferencingMetadata.DriverUrl.Should().Be("https://resolver.example.com");
    }

    [Fact]
    public void Success_WithDifferentContentTypes_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var content = "plain text content";
        var dereferencingMetadata = new DidDereferencingMetadata
        {
            ContentType = "text/plain",
            ResolutionTime = TimeSpan.FromMilliseconds(50)
        };

        // Act
        var result = DidDereferencingResult.Success(content, dereferencingMetadata);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Content.Should().Be(content);
        result.DereferencingMetadata.ContentType.Should().Be("text/plain");
    }

    [Fact]
    public void Success_WithJsonContent_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var content = new { message = "Hello, World!" };
        var dereferencingMetadata = new DidDereferencingMetadata
        {
            ContentType = "application/json",
            ResolutionTime = TimeSpan.FromMilliseconds(75)
        };

        // Act
        var result = DidDereferencingResult.Success(content, dereferencingMetadata);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Content.Should().Be(content);
        result.DereferencingMetadata.ContentType.Should().Be("application/json");
    }

    [Fact]
    public void Success_WithXmlContent_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var content = "<xml>content</xml>";
        var dereferencingMetadata = new DidDereferencingMetadata
        {
            ContentType = "application/xml",
            ResolutionTime = TimeSpan.FromMilliseconds(90)
        };

        // Act
        var result = DidDereferencingResult.Success(content, dereferencingMetadata);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Content.Should().Be(content);
        result.DereferencingMetadata.ContentType.Should().Be("application/xml");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Success_WithNullDereferencingMetadata_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var content = new { key = "value" };

        // Act
        var result = DidDereferencingResult.Success(content, null);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Content.Should().Be(content);
        result.DereferencingMetadata.Should().NotBeNull();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Success_WithEmptyContent_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var content = string.Empty;

        // Act
        var result = DidDereferencingResult.Success(content);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Content.Should().Be(content);
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Success_WithNullContent_ShouldCreateSuccessfulResult()
    {
        // Arrange
        string? content = null;

        // Act
        var result = DidDereferencingResult.Success(content!);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Content.Should().BeNull();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failed_WithLongErrorMessage_ShouldCreateFailedResult()
    {
        // Arrange
        var longErrorMessage = new string('A', 1000);

        // Act
        var result = DidDereferencingResult.Failed(longErrorMessage);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().Be(longErrorMessage);
    }

    [Fact]
    public void Failed_WithSpecialCharactersInErrorMessage_ShouldCreateFailedResult()
    {
        // Arrange
        var errorMessageWithSpecialChars = "Error: Invalid DID URL 'did:web:example.com#key-1' with special chars: !@#$%^&*()";

        // Act
        var result = DidDereferencingResult.Failed(errorMessageWithSpecialChars);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().Be(errorMessageWithSpecialChars);
    }

    [Fact]
    public void Equality_WithSameResult_ShouldBeEqual()
    {
        // Arrange
        var content = new { key = "value" };
        var dereferencingMetadata = new DidDereferencingMetadata
        {
            ContentType = "application/json",
            ResolutionTime = TimeSpan.FromMilliseconds(100)
        };

        var result1 = DidDereferencingResult.Success(content, dereferencingMetadata);
        var result2 = DidDereferencingResult.Success(content, dereferencingMetadata);

        // Act & Assert
        result1.Should().Be(result2);
        result1.GetHashCode().Should().Be(result2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentResult_ShouldNotBeEqual()
    {
        // Arrange
        var content1 = new { key = "value1" };
        var content2 = new { key = "value2" };

        var result1 = DidDereferencingResult.Success(content1);
        var result2 = DidDereferencingResult.Success(content2);

        // Act & Assert
        result1.Should().NotBe(result2);
    }

    [Fact]
    public void ToString_WithSuccessfulResult_ShouldReturnSuccessMessage()
    {
        // Arrange
        var content = new { key = "value" };
        var result = DidDereferencingResult.Success(content);

        // Act
        var resultString = result.ToString();

        // Assert
        resultString.Should().Contain("Success");
        resultString.Should().Contain("value");
    }

    [Fact]
    public void ToString_WithFailedResult_ShouldReturnErrorMessage()
    {
        // Arrange
        var errorMessage = "DID dereferencing failed";
        var result = DidDereferencingResult.Failed(errorMessage);

        // Act
        var resultString = result.ToString();

        // Assert
        resultString.Should().Contain("Failed");
        resultString.Should().Contain(errorMessage);
    }

    #endregion
}
