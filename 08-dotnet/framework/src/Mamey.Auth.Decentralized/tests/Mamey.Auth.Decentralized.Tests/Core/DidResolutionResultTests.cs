using FluentAssertions;
using Mamey.Auth.Decentralized.Core;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.Core;

/// <summary>
/// Tests for the DidResolutionResult class covering W3C DID 1.1 compliance.
/// </summary>
public class DidResolutionResultTests
{
    #region Happy Path Tests

    [Fact]
    public void Success_WithValidDidDocument_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };
        var resolutionMetadata = new DidResolutionMetadata
        {
            ContentType = "application/did+ld+json",
            ResolutionTime = TimeSpan.FromMilliseconds(100)
        };
        var documentMetadata = new DidDocumentMetadata
        {
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };

        // Act
        var result = DidResolutionResult.Success(didDocument, resolutionMetadata, documentMetadata);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.DidDocument.Should().Be(didDocument);
        result.ResolutionMetadata.Should().Be(resolutionMetadata);
        result.DocumentMetadata.Should().Be(documentMetadata);
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Success_WithMinimalParameters_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        // Act
        var result = DidResolutionResult.Success(didDocument);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.DidDocument.Should().Be(didDocument);
        result.ResolutionMetadata.Should().NotBeNull();
        result.DocumentMetadata.Should().NotBeNull();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failed_WithErrorMessage_ShouldCreateFailedResult()
    {
        // Arrange
        var errorMessage = "DID resolution failed";

        // Act
        var result = DidResolutionResult.Failed(errorMessage);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.DidDocument.Should().BeNull();
        result.ResolutionMetadata.Should().NotBeNull();
        result.DocumentMetadata.Should().BeNull();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public void InvalidDid_WithErrorMessage_ShouldCreateInvalidDidResult()
    {
        // Arrange
        var errorMessage = "Invalid DID format";

        // Act
        var result = DidResolutionResult.InvalidDid(errorMessage);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.DidDocument.Should().BeNull();
        result.ResolutionMetadata.Should().NotBeNull();
        result.DocumentMetadata.Should().BeNull();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public void MethodNotSupported_WithErrorMessage_ShouldCreateMethodNotSupportedResult()
    {
        // Arrange
        var errorMessage = "DID method not supported";

        // Act
        var result = DidResolutionResult.MethodNotSupported(errorMessage);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.DidDocument.Should().BeNull();
        result.ResolutionMetadata.Should().NotBeNull();
        result.DocumentMetadata.Should().BeNull();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public void NotFound_WithErrorMessage_ShouldCreateNotFoundResult()
    {
        // Arrange
        var errorMessage = "DID document not found";

        // Act
        var result = DidResolutionResult.NotFound(errorMessage);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.DidDocument.Should().BeNull();
        result.ResolutionMetadata.Should().NotBeNull();
        result.DocumentMetadata.Should().BeNull();
        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public void Deactivated_WithErrorMessage_ShouldCreateDeactivatedResult()
    {
        // Arrange
        var errorMessage = "DID has been deactivated";

        // Act
        var result = DidResolutionResult.Deactivated(errorMessage);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.DidDocument.Should().BeNull();
        result.ResolutionMetadata.Should().NotBeNull();
        result.DocumentMetadata.Should().BeNull();
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
        var action = () => DidResolutionResult.Failed(invalidErrorMessage);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void InvalidDid_WithInvalidErrorMessage_ShouldThrowArgumentException(string invalidErrorMessage)
    {
        // Act & Assert
        var action = () => DidResolutionResult.InvalidDid(invalidErrorMessage);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void MethodNotSupported_WithInvalidErrorMessage_ShouldThrowArgumentException(string invalidErrorMessage)
    {
        // Act & Assert
        var action = () => DidResolutionResult.MethodNotSupported(invalidErrorMessage);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void NotFound_WithInvalidErrorMessage_ShouldThrowArgumentException(string invalidErrorMessage)
    {
        // Act & Assert
        var action = () => DidResolutionResult.NotFound(invalidErrorMessage);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Deactivated_WithInvalidErrorMessage_ShouldThrowArgumentException(string invalidErrorMessage)
    {
        // Act & Assert
        var action = () => DidResolutionResult.Deactivated(invalidErrorMessage);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Success_WithNullDidDocument_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => DidResolutionResult.Success(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region W3C DID 1.1 Compliance Tests

    [Fact]
    public void Success_WithW3CCompliantDidDocument_ShouldCreateSuccessfulResult()
    {
        // Arrange - DID Document compliant with W3C DID 1.1 spec
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" },
            VerificationMethod = new List<VerificationMethod>
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
                }
            },
            Service = new List<ServiceEndpoint>
            {
                new()
                {
                    Id = "did:web:example.com#service-1",
                    Type = "DIDCommMessaging",
                    ServiceEndpointUrl = "https://example.com/messaging"
                }
            }
        };
        var resolutionMetadata = new DidResolutionMetadata
        {
            ContentType = "application/did+ld+json",
            ResolutionTime = TimeSpan.FromMilliseconds(100),
            Method = "web"
        };
        var documentMetadata = new DidDocumentMetadata
        {
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };

        // Act
        var result = DidResolutionResult.Success(didDocument, resolutionMetadata, documentMetadata);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.DidDocument.Should().Be(didDocument);
        result.ResolutionMetadata.Should().Be(resolutionMetadata);
        result.DocumentMetadata.Should().Be(documentMetadata);
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Success_WithResolutionMetadata_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };
        var resolutionMetadata = new DidResolutionMetadata
        {
            ContentType = "application/did+ld+json",
            ResolutionTime = TimeSpan.FromMilliseconds(100),
            Method = "web",
            Driver = "web-resolver",
            DriverUrl = "https://resolver.example.com"
        };

        // Act
        var result = DidResolutionResult.Success(didDocument, resolutionMetadata);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.ResolutionMetadata.Should().Be(resolutionMetadata);
        result.ResolutionMetadata.ContentType.Should().Be("application/did+ld+json");
        result.ResolutionMetadata.ResolutionTime.Should().Be(TimeSpan.FromMilliseconds(100));
        result.ResolutionMetadata.Method.Should().Be("web");
        result.ResolutionMetadata.Driver.Should().Be("web-resolver");
        result.ResolutionMetadata.DriverUrl.Should().Be("https://resolver.example.com");
    }

    [Fact]
    public void Success_WithDocumentMetadata_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };
        var documentMetadata = new DidDocumentMetadata
        {
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow,
            VersionId = "1",
            NextUpdate = DateTime.UtcNow.AddDays(1),
            NextVersionId = "2"
        };

        // Act
        var result = DidResolutionResult.Success(didDocument, null, documentMetadata);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.DocumentMetadata.Should().Be(documentMetadata);
        result.DocumentMetadata.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.DocumentMetadata.Updated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.DocumentMetadata.VersionId.Should().Be("1");
        result.DocumentMetadata.NextUpdate.Should().BeCloseTo(DateTime.UtcNow.AddDays(1), TimeSpan.FromSeconds(1));
        result.DocumentMetadata.NextVersionId.Should().Be("2");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Success_WithNullResolutionMetadata_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        // Act
        var result = DidResolutionResult.Success(didDocument, null, null);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.DidDocument.Should().Be(didDocument);
        result.ResolutionMetadata.Should().NotBeNull();
        result.DocumentMetadata.Should().NotBeNull();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Success_WithNullDocumentMetadata_ShouldCreateSuccessfulResult()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };
        var resolutionMetadata = new DidResolutionMetadata
        {
            ContentType = "application/did+ld+json",
            ResolutionTime = TimeSpan.FromMilliseconds(100)
        };

        // Act
        var result = DidResolutionResult.Success(didDocument, resolutionMetadata, null);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.DidDocument.Should().Be(didDocument);
        result.ResolutionMetadata.Should().Be(resolutionMetadata);
        result.DocumentMetadata.Should().NotBeNull();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failed_WithLongErrorMessage_ShouldCreateFailedResult()
    {
        // Arrange
        var longErrorMessage = new string('A', 1000);

        // Act
        var result = DidResolutionResult.Failed(longErrorMessage);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().Be(longErrorMessage);
    }

    [Fact]
    public void Failed_WithSpecialCharactersInErrorMessage_ShouldCreateFailedResult()
    {
        // Arrange
        var errorMessageWithSpecialChars = "Error: Invalid DID format 'did:web:example.com' with special chars: !@#$%^&*()";

        // Act
        var result = DidResolutionResult.Failed(errorMessageWithSpecialChars);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().Be(errorMessageWithSpecialChars);
    }

    [Fact]
    public void Equality_WithSameResult_ShouldBeEqual()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };
        var resolutionMetadata = new DidResolutionMetadata
        {
            ContentType = "application/did+ld+json",
            ResolutionTime = TimeSpan.FromMilliseconds(100)
        };
        var documentMetadata = new DidDocumentMetadata
        {
            Created = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };

        var result1 = DidResolutionResult.Success(didDocument, resolutionMetadata, documentMetadata);
        var result2 = DidResolutionResult.Success(didDocument, resolutionMetadata, documentMetadata);

        // Act & Assert
        result1.Should().Be(result2);
        result1.GetHashCode().Should().Be(result2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentResult_ShouldNotBeEqual()
    {
        // Arrange
        var didDocument1 = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };
        var didDocument2 = new DidDocument
        {
            Id = "did:web:different.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };

        var result1 = DidResolutionResult.Success(didDocument1);
        var result2 = DidResolutionResult.Success(didDocument2);

        // Act & Assert
        result1.Should().NotBe(result2);
    }

    [Fact]
    public void ToString_WithSuccessfulResult_ShouldReturnSuccessMessage()
    {
        // Arrange
        var didDocument = new DidDocument
        {
            Id = "did:web:example.com",
            Context = new List<string> { "https://www.w3.org/ns/did/v1" }
        };
        var result = DidResolutionResult.Success(didDocument);

        // Act
        var resultString = result.ToString();

        // Assert
        resultString.Should().Contain("Success");
        resultString.Should().Contain("did:web:example.com");
    }

    [Fact]
    public void ToString_WithFailedResult_ShouldReturnErrorMessage()
    {
        // Arrange
        var errorMessage = "DID resolution failed";
        var result = DidResolutionResult.Failed(errorMessage);

        // Act
        var resultString = result.ToString();

        // Assert
        resultString.Should().Contain("Failed");
        resultString.Should().Contain(errorMessage);
    }

    #endregion
}
