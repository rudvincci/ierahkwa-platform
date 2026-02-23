// File: Utils/B2BTokenAcquirer.cs
using Microsoft.Identity.Client;

namespace Mamey.Auth.Azure.B2B.Utilities;

/// <summary>
/// Utility class to handle token acquisition for B2B authentication.
/// </summary>
public class B2BTokenAcquirer
{
    private readonly IConfidentialClientApplication _app;

    /// <summary>
    /// Initializes the token acquirer with MSAL application.
    /// </summary>
    /// <param name="app">MSAL confidential client application.</param>
    public B2BTokenAcquirer(IConfidentialClientApplication app)
    {
        _app = app;
    }

    /// <summary>
    /// Acquires a token using client credentials for server-to-server authentication.
    /// </summary>
    /// <param name="scopes">The required scopes.</param>
    /// <returns>The access token.</returns>
    public async Task<string> GetTokenAsync(string[] scopes)
    {
        var result = await _app.AcquireTokenForClient(scopes).ExecuteAsync();
        return result.AccessToken;
    }
}
