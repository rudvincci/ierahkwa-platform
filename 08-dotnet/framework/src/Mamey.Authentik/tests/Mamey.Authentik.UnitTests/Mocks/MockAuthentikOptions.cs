using Microsoft.Extensions.Options;
using Mamey.Authentik;

namespace Mamey.Authentik.UnitTests.Mocks;

/// <summary>
/// Factory for creating mock Authentik options for testing.
/// </summary>
public static class MockAuthentikOptions
{
    /// <summary>
    /// Creates mock options with default test values.
    /// </summary>
    public static IOptions<AuthentikOptions> Create(string? baseUrl = null, string? apiToken = null)
    {
        var options = new AuthentikOptions
        {
            BaseUrl = baseUrl ?? "https://test.authentik.local",
            ApiToken = apiToken ?? "test-api-token",
            Timeout = TimeSpan.FromSeconds(30),
            ValidateSsl = false
        };

        return Options.Create(options);
    }

    /// <summary>
    /// Creates mock options with OAuth2 credentials.
    /// </summary>
    public static IOptions<AuthentikOptions> CreateWithOAuth2(
        string? baseUrl = null,
        string? clientId = null,
        string? clientSecret = null)
    {
        var options = new AuthentikOptions
        {
            BaseUrl = baseUrl ?? "https://test.authentik.local",
            ClientId = clientId ?? "test-client-id",
            ClientSecret = clientSecret ?? "test-client-secret",
            Timeout = TimeSpan.FromSeconds(30),
            ValidateSsl = false
        };

        return Options.Create(options);
    }
}
