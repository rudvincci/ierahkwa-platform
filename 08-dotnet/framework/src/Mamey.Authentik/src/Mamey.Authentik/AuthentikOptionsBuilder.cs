using Microsoft.Extensions.Logging;

namespace Mamey.Authentik;

/// <summary>
/// Fluent builder for configuring Authentik options.
/// </summary>
public class AuthentikOptionsBuilder
{
    private readonly AuthentikOptions _options = new();

    /// <summary>
    /// Sets the base URL of the Authentik instance.
    /// </summary>
    public AuthentikOptionsBuilder WithBaseUrl(string baseUrl)
    {
        _options.BaseUrl = baseUrl;
        return this;
    }

    /// <summary>
    /// Sets the API token for authentication.
    /// </summary>
    public AuthentikOptionsBuilder WithApiToken(string apiToken)
    {
        _options.ApiToken = apiToken;
        return this;
    }

    /// <summary>
    /// Sets OAuth2 client credentials for authentication.
    /// </summary>
    public AuthentikOptionsBuilder WithOAuth2Credentials(string clientId, string clientSecret, string? tokenEndpoint = null)
    {
        _options.ClientId = clientId;
        _options.ClientSecret = clientSecret;
        if (!string.IsNullOrWhiteSpace(tokenEndpoint))
        {
            _options.TokenEndpoint = tokenEndpoint;
        }
        return this;
    }

    /// <summary>
    /// Sets the HTTP request timeout.
    /// </summary>
    public AuthentikOptionsBuilder WithTimeout(TimeSpan timeout)
    {
        _options.Timeout = timeout;
        return this;
    }

    /// <summary>
    /// Sets whether to validate SSL certificates.
    /// </summary>
    public AuthentikOptionsBuilder WithSslValidation(bool validateSsl)
    {
        _options.ValidateSsl = validateSsl;
        return this;
    }

    /// <summary>
    /// Configures the retry policy.
    /// </summary>
    public AuthentikOptionsBuilder WithRetryPolicy(Action<RetryPolicyOptions> configure)
    {
        configure(_options.RetryPolicy);
        return this;
    }

    /// <summary>
    /// Configures caching options.
    /// </summary>
    public AuthentikOptionsBuilder WithCache(Action<CacheOptions> configure)
    {
        configure(_options.CacheOptions);
        return this;
    }

    /// <summary>
    /// Disables caching.
    /// </summary>
    public AuthentikOptionsBuilder WithoutCache()
    {
        _options.CacheOptions.Enabled = false;
        return this;
    }

    /// <summary>
    /// Sets the logging level for HTTP requests/responses.
    /// </summary>
    public AuthentikOptionsBuilder WithLogLevel(LogLevel logLevel)
    {
        _options.LogLevel = logLevel;
        return this;
    }

    /// <summary>
    /// Builds and validates the Authentik options.
    /// </summary>
    public AuthentikOptions Build()
    {
        _options.Validate();
        return _options;
    }
}
