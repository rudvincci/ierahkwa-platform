using Microsoft.Extensions.Logging;

namespace Mamey.Authentik;

/// <summary>
/// Configuration options for Authentik client.
/// </summary>
public class AuthentikOptions
{
    /// <summary>
    /// Gets or sets the base URL of the Authentik instance (e.g., https://authentik.company.com).
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the API token for authentication (required for API token auth).
    /// </summary>
    public string? ApiToken { get; set; }

    /// <summary>
    /// Gets or sets the OAuth2 client ID (optional, for OAuth2 client credentials flow).
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the OAuth2 client secret (optional, for OAuth2 client credentials flow).
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the OAuth2 token endpoint (optional, defaults to {BaseUrl}/application/o/token/).
    /// </summary>
    public string? TokenEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the HTTP request timeout (default: 30 seconds).
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets whether to validate SSL certificates (default: true).
    /// </summary>
    public bool ValidateSsl { get; set; } = true;

    /// <summary>
    /// Gets or sets the retry policy configuration.
    /// </summary>
    public RetryPolicyOptions RetryPolicy { get; set; } = new();

    /// <summary>
    /// Gets or sets the cache configuration.
    /// </summary>
    public CacheOptions CacheOptions { get; set; } = new();

    /// <summary>
    /// Gets or sets the logging level for HTTP requests/responses.
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Validates the configuration options.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when configuration is invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new InvalidOperationException("BaseUrl is required.");
        }

        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out var baseUri))
        {
            throw new InvalidOperationException($"BaseUrl '{BaseUrl}' is not a valid URI.");
        }

        if (baseUri.Scheme != "http" && baseUri.Scheme != "https")
        {
            throw new InvalidOperationException($"BaseUrl must use http or https scheme.");
        }

        // Validate authentication method
        var hasApiToken = !string.IsNullOrWhiteSpace(ApiToken);
        var hasOAuth2 = !string.IsNullOrWhiteSpace(ClientId) && !string.IsNullOrWhiteSpace(ClientSecret);

        if (!hasApiToken && !hasOAuth2)
        {
            throw new InvalidOperationException("Either ApiToken or ClientId/ClientSecret must be provided.");
        }

        if (Timeout <= TimeSpan.Zero)
        {
            throw new InvalidOperationException("Timeout must be greater than zero.");
        }
    }
}

/// <summary>
/// Retry policy configuration options.
/// </summary>
public class RetryPolicyOptions
{
    /// <summary>
    /// Gets or sets the maximum number of retry attempts (default: 3).
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets or sets the initial delay between retries (default: 1 second).
    /// </summary>
    public TimeSpan InitialDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the maximum delay between retries (default: 30 seconds).
    /// </summary>
    public TimeSpan MaxDelay { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets whether to use exponential backoff with jitter (default: true).
    /// </summary>
    public bool UseExponentialBackoff { get; set; } = true;
}

/// <summary>
/// Cache configuration options.
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// Gets or sets whether caching is enabled (default: true).
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the default cache TTL for GET requests (default: 5 minutes).
    /// </summary>
    public TimeSpan DefaultTtl { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the cache TTL for metadata endpoints like JWKS and OIDC discovery (default: 1 hour).
    /// </summary>
    public TimeSpan MetadataTtl { get; set; } = TimeSpan.FromHours(1);
}
