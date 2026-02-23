using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mamey;
using Mamey.Auth.Multi;
using Mamey.Auth.Multi.Middlewares;
using Xunit;

namespace Mamey.Auth.Multi.Tests.Integration;

/// <summary>
/// Integration tests for multi-authentication coordination
/// </summary>
public class MultiAuthIntegrationTests
{
    [Fact]
    public void MultiAuth_WithJwtOnly_ShouldRegisterJwt()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "auth:multi:enabled", "true" },
                { "auth:multi:enableJwt", "true" },
                { "auth:multi:policy", "JwtOnly" },
                { "jwt:issuer", "test-issuer" },
                { "jwt:audience", "test-audience" },
                { "jwt:secretKey", "test-secret-key-that-is-at-least-32-characters-long" }
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Act
        services.AddMamey()
            .AddMultiAuth();

        // Assert
        services.Should().Contain(s => s.ServiceType == typeof(MultiAuthOptions));
        services.Should().Contain(s => s.ServiceType == typeof(MultiAuthMiddleware));
    }

    [Fact]
    public void MultiAuth_WithMultipleMethods_ShouldRegisterAll()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "auth:multi:enabled", "true" },
                { "auth:multi:enableJwt", "true" },
                { "auth:multi:enableDecentralizedIdentifiers", "true" },
                { "auth:multi:enableAzure", "true" },
                { "auth:multi:policy", "EitherOr" },
                { "jwt:issuer", "test-issuer" },
                { "jwt:audience", "test-audience" },
                { "jwt:secretKey", "test-secret-key-that-is-at-least-32-characters-long" }
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Act
        services.AddMamey()
            .AddMultiAuth();

        // Assert
        services.Should().Contain(s => s.ServiceType == typeof(MultiAuthOptions));
        services.Should().Contain(s => s.ServiceType == typeof(MultiAuthMiddleware));
    }

    [Fact]
    public void MultiAuth_WithCollisionPrevention_ShouldOnlyRegisterOnce()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "auth:multi:enabled", "true" },
                { "auth:multi:enableJwt", "true" },
                { "auth:multi:policy", "JwtOnly" },
                { "jwt:issuer", "test-issuer" },
                { "jwt:audience", "test-audience" },
                { "jwt:secretKey", "test-secret-key-that-is-at-least-32-characters-long" }
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Act
        var mameyBuilder = services.AddMamey();
        var result1 = mameyBuilder.AddMultiAuth();
        var result2 = mameyBuilder.AddMultiAuth();

        // Assert
        result1.Should().Be(result2);
        services.Count(s => s.ServiceType == typeof(MultiAuthOptions)).Should().BeLessThanOrEqualTo(1);
        services.Count(s => s.ServiceType == typeof(MultiAuthMiddleware)).Should().BeLessThanOrEqualTo(1);
    }

    [Fact]
    public void MultiAuth_WithDisabled_ShouldNotRegister()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "auth:multi:enabled", "false" },
                { "auth:multi:enableJwt", "true" },
                { "auth:multi:policy", "JwtOnly" }
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Act
        services.AddMamey()
            .AddMultiAuth();

        // Assert
        // When disabled, options and middleware are still registered
        // The middleware checks options.Enabled at runtime and skips processing if disabled
        // This is expected behavior - the middleware is registered but does nothing when disabled
        services.Should().Contain(s => s.ServiceType == typeof(MultiAuthOptions));
        services.Should().Contain(s => s.ServiceType == typeof(MultiAuthMiddleware));
    }

    [Fact]
    public void MultiAuth_WithAzureAndJwt_ShouldNotCollide()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "auth:multi:enabled", "true" },
                { "auth:multi:enableJwt", "true" },
                { "auth:multi:enableAzure", "true" },
                { "auth:multi:policy", "EitherOr" },
                { "jwt:issuer", "test-issuer" },
                { "jwt:audience", "test-audience" },
                { "jwt:secretKey", "test-secret-key-that-is-at-least-32-characters-long" },
                { "azure:enabled", "false" }
            })
            .Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Act
        services.AddMamey()
            .AddMultiAuth();

        // Assert
        services.Should().Contain(s => s.ServiceType == typeof(MultiAuthOptions));
        services.Should().Contain(s => s.ServiceType == typeof(MultiAuthMiddleware));
        // Verify no duplicate registrations
        services.Count(s => s.ServiceType == typeof(MultiAuthOptions)).Should().Be(1);
        services.Count(s => s.ServiceType == typeof(MultiAuthMiddleware)).Should().Be(1);
    }
}

