namespace Mamey.Auth.Decentralized.Exceptions;

/// <summary>
/// Exception thrown when a DID method is not supported
/// </summary>
public class DidMethodNotSupportedException : DidException
{
    /// <summary>
    /// The unsupported DID method
    /// </summary>
    public string Method { get; }
    
    /// <summary>
    /// Initializes a new instance of the DidMethodNotSupportedException class
    /// </summary>
    public DidMethodNotSupportedException() : base()
    {
        Method = string.Empty;
    }
    
    /// <summary>
    /// Initializes a new instance of the DidMethodNotSupportedException class with a specified error message
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public DidMethodNotSupportedException(string message) : base(message)
    {
        Method = string.Empty;
    }
    
    /// <summary>
    /// Initializes a new instance of the DidMethodNotSupportedException class with a specified error message and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public DidMethodNotSupportedException(string message, Exception innerException) : base(message, innerException)
    {
        Method = string.Empty;
    }
    
    /// <summary>
    /// Initializes a new instance of the DidMethodNotSupportedException class with a specified method and error message
    /// </summary>
    /// <param name="method">The unsupported DID method</param>
    /// <param name="message">The message that describes the error</param>
    public DidMethodNotSupportedException(string method, string message) : base(message)
    {
        Method = method;
    }
    
    /// <summary>
    /// Initializes a new instance of the DidMethodNotSupportedException class with a specified method, error message, and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="method">The unsupported DID method</param>
    /// <param name="message">The error message that explains the reason for the exception</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public DidMethodNotSupportedException(string method, string message, Exception innerException) : base(message, innerException)
    {
        Method = method;
    }
}
