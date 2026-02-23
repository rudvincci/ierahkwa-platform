using FluentAssertions;
using Mamey.Authentik;
using Xunit;

namespace Mamey.Authentik.UnitTests;

public class AuthentikOptionsTests
{
    [Fact]
    public void Validate_WithValidApiToken_ShouldNotThrow()
    {
        // Arrange
        var options = new AuthentikOptions
        {
            BaseUrl = "https://test.authentik.local",
            ApiToken = "test-token"
        };

        // Act & Assert
        options.Invoking(o => o.Validate()).Should().NotThrow();
    }

    [Fact]
    public void Validate_WithValidOAuth2Credentials_ShouldNotThrow()
    {
        // Arrange
        var options = new AuthentikOptions
        {
            BaseUrl = "https://test.authentik.local",
            ClientId = "test-client",
            ClientSecret = "test-secret"
        };

        // Act & Assert
        options.Invoking(o => o.Validate()).Should().NotThrow();
    }

    [Fact]
    public void Validate_WithEmptyBaseUrl_ShouldThrow()
    {
        // Arrange
        var options = new AuthentikOptions
        {
            BaseUrl = string.Empty,
            ApiToken = "test-token"
        };

        // Act & Assert
        options.Invoking(o => o.Validate())
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*BaseUrl is required*");
    }

    [Fact]
    public void Validate_WithInvalidBaseUrl_ShouldThrow()
    {
        // Arrange
        var options = new AuthentikOptions
        {
            BaseUrl = "not-a-valid-url",
            ApiToken = "test-token"
        };

        // Act & Assert
        options.Invoking(o => o.Validate())
            .Should()
            .Throw<InvalidOperationException>();
    }

    [Fact]
    public void Validate_WithNoAuthentication_ShouldThrow()
    {
        // Arrange
        var options = new AuthentikOptions
        {
            BaseUrl = "https://test.authentik.local"
        };

        // Act & Assert
        options.Invoking(o => o.Validate())
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*ApiToken or ClientId/ClientSecret*");
    }
}
