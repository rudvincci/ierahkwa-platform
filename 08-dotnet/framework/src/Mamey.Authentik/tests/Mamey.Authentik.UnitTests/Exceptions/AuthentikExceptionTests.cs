using FluentAssertions;
using Mamey.Authentik.Exceptions;
using Xunit;

namespace Mamey.Authentik.UnitTests.Exceptions;

public class AuthentikExceptionTests
{
    [Fact]
    public void AuthentikApiException_ShouldContainStatusCode()
    {
        // Act
        var exception = new AuthentikApiException(404, "Not found", "response body", "https://test.local/api");

        // Assert
        exception.StatusCode.Should().Be(404);
        exception.Message.Should().Contain("Not found");
        exception.ResponseBody.Should().Be("response body");
        exception.RequestUri.Should().Be("https://test.local/api");
    }

    [Fact]
    public void AuthentikValidationException_ShouldContainValidationErrors()
    {
        // Arrange
        var errors = new Dictionary<string, string[]>
        {
            { "email", new[] { "Invalid email format" } },
            { "username", new[] { "Username is required" } }
        };

        // Act
        var exception = new AuthentikValidationException("Validation failed", errors);

        // Assert
        exception.StatusCode.Should().Be(400);
        exception.ValidationErrors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void AuthentikRateLimitException_ShouldContainRetryAfter()
    {
        // Act
        var exception = new AuthentikRateLimitException("Rate limited", 60);

        // Assert
        exception.StatusCode.Should().Be(429);
        exception.RetryAfterSeconds.Should().Be(60);
    }
}
