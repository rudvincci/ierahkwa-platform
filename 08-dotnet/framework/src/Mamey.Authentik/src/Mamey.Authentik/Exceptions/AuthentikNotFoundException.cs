namespace Mamey.Authentik.Exceptions;

/// <summary>
/// Exception thrown when a resource is not found (404 Not Found).
/// </summary>
public class AuthentikNotFoundException : AuthentikApiException
{
    /// <summary>
    /// Gets the resource identifier that was not found.
    /// </summary>
    public string? ResourceId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikNotFoundException"/> class.
    /// </summary>
    public AuthentikNotFoundException(string message, string? resourceId = null, string? responseBody = null, string? requestUri = null)
        : base(404, message, responseBody, requestUri)
    {
        ResourceId = resourceId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikNotFoundException"/> class with an inner exception.
    /// </summary>
    public AuthentikNotFoundException(string message, Exception innerException, string? resourceId = null, string? responseBody = null, string? requestUri = null)
        : base(404, message, innerException, responseBody, requestUri)
    {
        ResourceId = resourceId;
    }
}
