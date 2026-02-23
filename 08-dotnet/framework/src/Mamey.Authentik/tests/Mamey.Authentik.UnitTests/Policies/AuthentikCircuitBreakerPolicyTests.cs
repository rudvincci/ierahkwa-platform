using FluentAssertions;
using Mamey.Authentik.Policies;
using Xunit;

namespace Mamey.Authentik.UnitTests.Policies;

public class AuthentikCircuitBreakerPolicyTests
{
    [Fact]
    public void CreatePolicy_WithDefaultSettings_CreatesCircuitBreaker()
    {
        // Act
        var policy = AuthentikCircuitBreakerPolicy.CreatePolicy();

        // Assert
        policy.Should().NotBeNull();
    }

    [Fact]
    public void CreatePolicy_WithCustomSettings_CreatesCircuitBreaker()
    {
        // Act
        var policy = AuthentikCircuitBreakerPolicy.CreatePolicy(
            handledEventsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(60));

        // Assert
        policy.Should().NotBeNull();
    }
}
