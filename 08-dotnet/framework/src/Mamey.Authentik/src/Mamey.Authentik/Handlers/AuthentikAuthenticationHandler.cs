using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.Authentik.Handlers;

/// <summary>
/// HTTP message handler that adds authentication to requests.
/// </summary>
public class AuthentikAuthenticationHandler : DelegatingHandler
{
    private readonly AuthentikOptions _options;
    private readonly ILogger<AuthentikAuthenticationHandler> _logger;
    private string? _cachedToken;
    private DateTimeOffset? _tokenExpiresAt;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikAuthenticationHandler"/> class.
    /// </summary>
    public AuthentikAuthenticationHandler(
        IOptions<AuthentikOptions> options,
        ILogger<AuthentikAuthenticationHandler> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Add authentication header
        if (!string.IsNullOrWhiteSpace(_options.ApiToken))
        {
            // API token authentication
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiToken);
        }
        else if (!string.IsNullOrWhiteSpace(_options.ClientId) && !string.IsNullOrWhiteSpace(_options.ClientSecret))
        {
            // OAuth2 client credentials flow
            var token = await GetOAuth2TokenAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string> GetOAuth2TokenAsync(CancellationToken cancellationToken)
    {
        // Check if we have a valid cached token
        if (!string.IsNullOrWhiteSpace(_cachedToken) && _tokenExpiresAt.HasValue && _tokenExpiresAt > DateTimeOffset.UtcNow.AddMinutes(5))
        {
            return _cachedToken;
        }

        // Fetch new token
        var tokenEndpoint = _options.TokenEndpoint ?? $"{_options.BaseUrl.TrimEnd('/')}/application/o/token/";
        
        using var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
        request.Content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", _options.ClientId!),
            new KeyValuePair<string, string>("client_secret", _options.ClientSecret!),
            new KeyValuePair<string, string>("scope", "goauthentik.io/api")
        });

        using var httpClient = new HttpClient();
        var response = await httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Failed to obtain OAuth2 token. Status: {StatusCode}, Body: {Body}", 
                response.StatusCode, errorBody);
            throw new Exceptions.AuthentikAuthenticationException(
                $"Failed to obtain OAuth2 token: {response.StatusCode}",
                errorBody,
                tokenEndpoint);
        }

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (tokenResponse == null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
        {
            throw new Exceptions.AuthentikAuthenticationException(
                "Invalid token response: access_token is missing",
                responseBody,
                tokenEndpoint);
        }

        // Cache the token
        _cachedToken = tokenResponse.AccessToken;
        _tokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60); // Refresh 1 minute before expiry

        _logger.LogDebug("Obtained new OAuth2 token, expires at {ExpiresAt}", _tokenExpiresAt);

        return _cachedToken;
    }

    private class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string? TokenType { get; set; }
    }
}
