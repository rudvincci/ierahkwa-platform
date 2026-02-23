using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mamey.Authentik;
using Xunit;

namespace Mamey.Authentik.UnitTests;

public class AuthentikOptionsBuilderTests
{
    [Fact]
    public void WithBaseUrl_SetsBaseUrl()
    {
        // Act
        var options = new AuthentikOptionsBuilder()
            .WithBaseUrl("https://test.authentik.local")
            .WithApiToken("test-token")
            .Build();

        // Assert
        options.BaseUrl.Should().Be("https://test.authentik.local");
    }

    [Fact]
    public void WithApiToken_SetsApiToken()
    {
        // Act
        var options = new AuthentikOptionsBuilder()
            .WithBaseUrl("https://test.authentik.local")
            .WithApiToken("test-token")
            .Build();

        // Assert
        options.ApiToken.Should().Be("test-token");
    }

    [Fact]
    public void WithOAuth2Credentials_SetsOAuth2Credentials()
    {
        // Act
        var options = new AuthentikOptionsBuilder()
            .WithBaseUrl("https://test.authentik.local")
            .WithOAuth2Credentials("client-id", "client-secret")
            .Build();

        // Assert
        options.ClientId.Should().Be("client-id");
        options.ClientSecret.Should().Be("client-secret");
    }

    [Fact]
    public void WithTimeout_SetsTimeout()
    {
        // Act
        var timeout = TimeSpan.FromSeconds(60);
        var options = new AuthentikOptionsBuilder()
            .WithBaseUrl("https://test.authentik.local")
            .WithApiToken("test-token")
            .WithTimeout(timeout)
            .Build();

        // Assert
        options.Timeout.Should().Be(timeout);
    }

    [Fact]
    public void WithRetryPolicy_ConfiguresRetryPolicy()
    {
        // Act
        var options = new AuthentikOptionsBuilder()
            .WithBaseUrl("https://test.authentik.local")
            .WithApiToken("test-token")
            .WithRetryPolicy(policy =>
            {
                policy.MaxRetries = 5;
                policy.InitialDelay = TimeSpan.FromSeconds(2);
            })
            .Build();

        // Assert
        options.RetryPolicy.MaxRetries.Should().Be(5);
        options.RetryPolicy.InitialDelay.Should().Be(TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void WithCache_ConfiguresCache()
    {
        // Act
        var options = new AuthentikOptionsBuilder()
            .WithBaseUrl("https://test.authentik.local")
            .WithApiToken("test-token")
            .WithCache(cache =>
            {
                cache.Enabled = true;
                cache.DefaultTtl = TimeSpan.FromMinutes(10);
            })
            .Build();

        // Assert
        options.CacheOptions.Enabled.Should().BeTrue();
        options.CacheOptions.DefaultTtl.Should().Be(TimeSpan.FromMinutes(10));
    }

    [Fact]
    public void WithoutCache_DisablesCache()
    {
        // Act
        var options = new AuthentikOptionsBuilder()
            .WithBaseUrl("https://test.authentik.local")
            .WithApiToken("test-token")
            .WithoutCache()
            .Build();

        // Assert
        options.CacheOptions.Enabled.Should().BeFalse();
    }

    [Fact]
    public void WithLogLevel_SetsLogLevel()
    {
        // Act
        var options = new AuthentikOptionsBuilder()
            .WithBaseUrl("https://test.authentik.local")
            .WithApiToken("test-token")
            .WithLogLevel(LogLevel.Debug)
            .Build();

        // Assert
        options.LogLevel.Should().Be(LogLevel.Debug);
    }

    [Fact]
    public void Build_WithInvalidConfiguration_ThrowsException()
    {
        // Act & Assert
        var builder = new AuthentikOptionsBuilder()
            .WithBaseUrl("invalid-url");

        builder.Invoking(b => b.Build())
            .Should()
            .Throw<InvalidOperationException>();
    }
}
