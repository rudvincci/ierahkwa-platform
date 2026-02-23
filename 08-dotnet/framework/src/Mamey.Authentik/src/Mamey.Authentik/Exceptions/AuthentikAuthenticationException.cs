namespace Mamey.Authentik.Exceptions;

/// <summary>
/// Exception thrown when authentication fails (401 Unauthorized).
/// </summary>
public class AuthentikAuthenticationException : AuthentikApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikAuthenticationException"/> class.
    /// </summary>
    public AuthentikAuthenticationException(string message, string? responseBody = null, string? requestUri = null)
        : base(401, message, responseBody, requestUri)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikAuthenticationException"/> class with an inner exception.
    /// </summary>
    public AuthentikAuthenticationException(string message, Exception innerException, string? responseBody = null, string? requestUri = null)
        : base(401, message, innerException, responseBody, requestUri)
    {
    }
}
