using Microsoft.Extensions.DependencyInjection;
using Mamey.Authentik;
using Mamey.Authentik.IntegrationTests.TestFixtures;

namespace Mamey.Authentik.IntegrationTests.TestFixtures;

/// <summary>
/// Helper class for integration tests to create configured clients.
/// </summary>
public static class IntegrationTestHelper
{
    /// <summary>
    /// Creates a configured Authentik client for testing.
    /// </summary>
    public static IAuthentikClient? CreateClient(AuthentikTestFixture fixture)
    {
        if (!fixture.IsContainerRunning || string.IsNullOrWhiteSpace(fixture.ApiToken))
        {
            return null;
        }

        var services = new ServiceCollection();
        services.AddAuthentik(options =>
        {
            options.BaseUrl = fixture.BaseUrl;
            options.ApiToken = fixture.ApiToken;
            options.Timeout = TimeSpan.FromSeconds(30);
        });

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IAuthentikClient>();
    }

    /// <summary>
    /// Creates a client with invalid token for testing error scenarios.
    /// </summary>
    public static IAuthentikClient CreateClientWithInvalidToken(string baseUrl)
    {
        var services = new ServiceCollection();
        services.AddAuthentik(options =>
        {
            options.BaseUrl = baseUrl;
            options.ApiToken = "invalid-token-12345";
            options.Timeout = TimeSpan.FromSeconds(5);
        });

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IAuthentikClient>();
    }

    /// <summary>
    /// Creates a client with invalid base URL for testing error scenarios.
    /// </summary>
    public static IAuthentikClient CreateClientWithInvalidUrl()
    {
        var services = new ServiceCollection();
        services.AddAuthentik(options =>
        {
            options.BaseUrl = "http://invalid-url-that-does-not-exist:9999";
            options.ApiToken = "test-token";
            options.Timeout = TimeSpan.FromSeconds(5);
        });

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<IAuthentikClient>();
    }
}
