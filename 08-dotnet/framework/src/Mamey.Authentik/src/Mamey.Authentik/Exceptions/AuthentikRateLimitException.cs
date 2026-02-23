namespace Mamey.Authentik.Exceptions;

/// <summary>
/// Exception thrown when rate limit is exceeded (429 Too Many Requests).
/// </summary>
public class AuthentikRateLimitException : AuthentikApiException
{
    /// <summary>
    /// Gets the retry-after value in seconds, if available.
    /// </summary>
    public int? RetryAfterSeconds { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikRateLimitException"/> class.
    /// </summary>
    public AuthentikRateLimitException(string message, int? retryAfterSeconds = null, string? responseBody = null, string? requestUri = null)
        : base(429, message, responseBody, requestUri)
    {
        RetryAfterSeconds = retryAfterSeconds;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikRateLimitException"/> class with an inner exception.
    /// </summary>
    public AuthentikRateLimitException(string message, Exception innerException, int? retryAfterSeconds = null, string? responseBody = null, string? requestUri = null)
        : base(429, message, innerException, responseBody, requestUri)
    {
        RetryAfterSeconds = retryAfterSeconds;
    }
}
