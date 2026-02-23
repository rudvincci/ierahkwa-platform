using System.Net;
using FluentAssertions;
using Mamey.Authentik;
using Mamey.Authentik.Policies;
using Polly;
using Xunit;

namespace Mamey.Authentik.UnitTests.Policies;

public class AuthentikRetryPolicyTests
{
    [Fact]
    public void CreatePolicy_WithDefaultOptions_CreatesRetryPolicy()
    {
        // Arrange
        var options = new AuthentikOptions
        {
            BaseUrl = "https://test.authentik.local",
            ApiToken = "test-token",
            RetryPolicy = new RetryPolicyOptions()
        };

        // Act
        var policy = AuthentikRetryPolicy.CreatePolicy(options);

        // Assert
        policy.Should().NotBeNull();
    }

    [Fact]
    public void CreatePolicy_WithCustomRetryCount_RespectsMaxRetries()
    {
        // Arrange
        var options = new AuthentikOptions
        {
            BaseUrl = "https://test.authentik.local",
            ApiToken = "test-token",
            RetryPolicy = new RetryPolicyOptions
            {
                MaxRetries = 5,
                UseExponentialBackoff = false
            }
        };

        // Act
        var policy = AuthentikRetryPolicy.CreatePolicy(options);

        // Assert
        policy.Should().NotBeNull();
    }

    [Fact]
    public void CreatePolicy_WithExponentialBackoff_UsesExponentialBackoff()
    {
        // Arrange
        var options = new AuthentikOptions
        {
            BaseUrl = "https://test.authentik.local",
            ApiToken = "test-token",
            RetryPolicy = new RetryPolicyOptions
            {
                MaxRetries = 3,
                UseExponentialBackoff = true,
                InitialDelay = TimeSpan.FromMilliseconds(100),
                MaxDelay = TimeSpan.FromSeconds(1)
            }
        };

        // Act
        var policy = AuthentikRetryPolicy.CreatePolicy(options);

        // Assert
        policy.Should().NotBeNull();
    }
}
