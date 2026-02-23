namespace Mamey.Authentik.Exceptions;

/// <summary>
/// Exception thrown when an API request fails.
/// </summary>
public class AuthentikApiException : AuthentikException
{
    /// <summary>
    /// Gets the HTTP status code of the response.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Gets the response body, if available.
    /// </summary>
    public string? ResponseBody { get; }

    /// <summary>
    /// Gets the request URI that failed.
    /// </summary>
    public string? RequestUri { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikApiException"/> class.
    /// </summary>
    public AuthentikApiException(int statusCode, string message, string? responseBody = null, string? requestUri = null)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
        RequestUri = requestUri;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikApiException"/> class with an inner exception.
    /// </summary>
    public AuthentikApiException(int statusCode, string message, Exception innerException, string? responseBody = null, string? requestUri = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
        RequestUri = requestUri;
    }
}
