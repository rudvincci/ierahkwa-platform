namespace Mamey.Auth.Decentralized.Exceptions;

/// <summary>
/// Exception thrown when a DID method is not supported.
/// </summary>
public class UnsupportedDidMethodException : DidException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedDidMethodException"/> class.
    /// </summary>
    public UnsupportedDidMethodException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedDidMethodException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UnsupportedDidMethodException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedDidMethodException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public UnsupportedDidMethodException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsupportedDidMethodException"/> class with a specified DID method and error message.
    /// </summary>
    /// <param name="method">The unsupported DID method.</param>
    /// <param name="message">The message that describes the error.</param>
    public UnsupportedDidMethodException(string method, string message) : base($"Unsupported DID method '{method}': {message}")
    {
        Method = method;
    }

    /// <summary>
    /// Gets the unsupported DID method.
    /// </summary>
    public string? Method { get; }
}
