namespace Mamey.Authentik.Exceptions;

/// <summary>
/// Exception thrown when authorization fails (403 Forbidden).
/// </summary>
public class AuthentikAuthorizationException : AuthentikApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikAuthorizationException"/> class.
    /// </summary>
    public AuthentikAuthorizationException(string message, string? responseBody = null, string? requestUri = null)
        : base(403, message, responseBody, requestUri)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikAuthorizationException"/> class with an inner exception.
    /// </summary>
    public AuthentikAuthorizationException(string message, Exception innerException, string? responseBody = null, string? requestUri = null)
        : base(403, message, innerException, responseBody, requestUri)
    {
    }
}
