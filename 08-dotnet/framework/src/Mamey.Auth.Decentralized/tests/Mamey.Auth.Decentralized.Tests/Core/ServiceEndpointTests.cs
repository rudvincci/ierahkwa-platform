using FluentAssertions;
using Mamey.Auth.Decentralized.Core;
using Mamey.Auth.Decentralized.Exceptions;
using Xunit;

namespace Mamey.Auth.Decentralized.Tests.Core;

/// <summary>
/// Tests for the ServiceEndpoint class covering W3C DID 1.1 compliance.
/// </summary>
public class ServiceEndpointTests
{
    #region Happy Path Tests

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateServiceEndpoint()
    {
        // Arrange
        var id = "did:web:example.com#service-1";
        var type = "DIDCommMessaging";
        var serviceEndpointUrl = "https://example.com/messaging";

        // Act
        var serviceEndpoint = new ServiceEndpoint
        {
            Id = id,
            Type = type,
            ServiceEndpointUrl = serviceEndpointUrl
        };

        // Assert
        serviceEndpoint.Id.Should().Be(id);
        serviceEndpoint.Type.Should().Be(type);
        serviceEndpoint.ServiceEndpointUrl.Should().Be(serviceEndpointUrl);
    }

    [Theory]
    [InlineData("DIDCommMessaging")]
    [InlineData("LinkedDomains")]
    [InlineData("DIDCommV2")]
    [InlineData("OIDCProvider")]
    [InlineData("OIDCClient")]
    public void Constructor_WithSupportedServiceTypes_ShouldCreateServiceEndpoint(string type)
    {
        // Arrange
        var id = "did:web:example.com#service-1";
        var serviceEndpointUrl = "https://example.com/service";

        // Act
        var serviceEndpoint = new ServiceEndpoint
        {
            Id = id,
            Type = type,
            ServiceEndpointUrl = serviceEndpointUrl
        };

        // Assert
        serviceEndpoint.Type.Should().Be(type);
    }

    [Fact]
    public void Constructor_WithValidUrl_ShouldCreateServiceEndpoint()
    {
        // Arrange
        var id = "did:web:example.com#service-1";
        var type = "DIDCommMessaging";
        var serviceEndpointUrl = "https://example.com/messaging";

        // Act
        var serviceEndpoint = new ServiceEndpoint
        {
            Id = id,
            Type = type,
            ServiceEndpointUrl = serviceEndpointUrl
        };

        // Assert
        serviceEndpoint.ServiceEndpointUrl.Should().Be(serviceEndpointUrl);
    }

    [Fact]
    public void Constructor_WithProperties_ShouldCreateServiceEndpoint()
    {
        // Arrange
        var id = "did:web:example.com#service-1";
        var type = "DIDCommMessaging";
        var serviceEndpointUrl = "https://example.com/messaging";
        var properties = new Dictionary<string, JsonElement>
        {
            ["routingKeys"] = JsonSerializer.SerializeToElement(new[] { "key1", "key2" }),
            ["accept"] = JsonSerializer.SerializeToElement(new[] { "application/didcomm+json" })
        };

        // Act
        var serviceEndpoint = new ServiceEndpoint
        {
            Id = id,
            Type = type,
            ServiceEndpointUrl = serviceEndpointUrl,
            Properties = properties
        };

        // Assert
        serviceEndpoint.Properties.Should().BeEquivalentTo(properties);
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
        var action = () => new ServiceEndpoint
        {
            Id = invalidId,
            Type = "DIDCommMessaging",
            ServiceEndpointUrl = "https://example.com/messaging"
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
        var action = () => new ServiceEndpoint
        {
            Id = "did:web:example.com#service-1",
            Type = invalidType,
            ServiceEndpointUrl = "https://example.com/messaging"
        };

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("invalid-url")]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com")]
    public void Constructor_WithInvalidServiceEndpointUrl_ShouldThrowArgumentException(string invalidUrl)
    {
        // Act & Assert
        var action = () => new ServiceEndpoint
        {
            Id = "did:web:example.com#service-1",
            Type = "DIDCommMessaging",
            ServiceEndpointUrl = invalidUrl
        };

        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region W3C DID 1.1 Compliance Tests

    [Fact]
    public void Constructor_WithW3CCompliantService_ShouldCreateServiceEndpoint()
    {
        // Arrange - Service compliant with W3C DID 1.1 spec
        var id = "did:web:example.com#service-1";
        var type = "DIDCommMessaging";
        var serviceEndpointUrl = "https://example.com/messaging";

        // Act
        var serviceEndpoint = new ServiceEndpoint
        {
            Id = id,
            Type = type,
            ServiceEndpointUrl = serviceEndpointUrl
        };

        // Assert
        serviceEndpoint.Id.Should().Be(id);
        serviceEndpoint.Type.Should().Be(type);
        serviceEndpoint.ServiceEndpointUrl.Should().Be(serviceEndpointUrl);
    }

    [Fact]
    public void Constructor_WithLinkedDomainsService_ShouldCreateServiceEndpoint()
    {
        // Arrange
        var id = "did:web:example.com#linked-domains";
        var type = "LinkedDomains";
        var serviceEndpointUrl = "https://example.com";

        // Act
        var serviceEndpoint = new ServiceEndpoint
        {
            Id = id,
            Type = type,
            ServiceEndpointUrl = serviceEndpointUrl
        };

        // Assert
        serviceEndpoint.Type.Should().Be("LinkedDomains");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Constructor_WithSpecialCharactersInId_ShouldCreateServiceEndpoint()
    {
        // Arrange
        var id = "did:web:example.com#service-with-special-chars_123";
        var type = "DIDCommMessaging";
        var serviceEndpointUrl = "https://example.com/messaging";

        // Act
        var serviceEndpoint = new ServiceEndpoint
        {
            Id = id,
            Type = type,
            ServiceEndpointUrl = serviceEndpointUrl
        };

        // Assert
        serviceEndpoint.Id.Should().Be(id);
    }

    [Fact]
    public void Constructor_WithComplexUrl_ShouldCreateServiceEndpoint()
    {
        // Arrange
        var id = "did:web:example.com#service-1";
        var type = "DIDCommMessaging";
        var serviceEndpointUrl = "https://example.com:8080/messaging/v1?param=value#fragment";

        // Act
        var serviceEndpoint = new ServiceEndpoint
        {
            Id = id,
            Type = type,
            ServiceEndpointUrl = serviceEndpointUrl
        };

        // Assert
        serviceEndpoint.ServiceEndpointUrl.Should().Be(serviceEndpointUrl);
    }

    [Fact]
    public void Equality_WithSameServiceEndpoint_ShouldBeEqual()
    {
        // Arrange
        var id = "did:web:example.com#service-1";
        var type = "DIDCommMessaging";
        var serviceEndpointUrl = "https://example.com/messaging";

        var serviceEndpoint1 = new ServiceEndpoint
        {
            Id = id,
            Type = type,
            ServiceEndpointUrl = serviceEndpointUrl
        };

        var serviceEndpoint2 = new ServiceEndpoint
        {
            Id = id,
            Type = type,
            ServiceEndpointUrl = serviceEndpointUrl
        };

        // Act & Assert
        serviceEndpoint1.Should().Be(serviceEndpoint2);
        serviceEndpoint1.GetHashCode().Should().Be(serviceEndpoint2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentServiceEndpoint_ShouldNotBeEqual()
    {
        // Arrange
        var id = "did:web:example.com#service-1";
        var type = "DIDCommMessaging";
        var serviceEndpointUrl = "https://example.com/messaging";

        var serviceEndpoint1 = new ServiceEndpoint
        {
            Id = id,
            Type = type,
            ServiceEndpointUrl = serviceEndpointUrl
        };

        var serviceEndpoint2 = new ServiceEndpoint
        {
            Id = "did:web:example.com#service-2",
            Type = type,
            ServiceEndpointUrl = serviceEndpointUrl
        };

        // Act & Assert
        serviceEndpoint1.Should().NotBe(serviceEndpoint2);
    }

    #endregion
}
