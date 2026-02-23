namespace Mamey.Authentik.Exceptions;

/// <summary>
/// Exception thrown when request validation fails (400 Bad Request).
/// </summary>
public class AuthentikValidationException : AuthentikApiException
{
    /// <summary>
    /// Gets the validation errors, if available.
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikValidationException"/> class.
    /// </summary>
    public AuthentikValidationException(string message, Dictionary<string, string[]>? validationErrors = null, string? responseBody = null, string? requestUri = null)
        : base(400, message, responseBody, requestUri)
    {
        ValidationErrors = validationErrors;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthentikValidationException"/> class with an inner exception.
    /// </summary>
    public AuthentikValidationException(string message, Exception innerException, Dictionary<string, string[]>? validationErrors = null, string? responseBody = null, string? requestUri = null)
        : base(400, message, innerException, responseBody, requestUri)
    {
        ValidationErrors = validationErrors;
    }
}
