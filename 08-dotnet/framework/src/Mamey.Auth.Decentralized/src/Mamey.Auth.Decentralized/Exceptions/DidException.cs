namespace Mamey.Auth.Decentralized.Exceptions;

/// <summary>
/// Base exception for DID-related operations
/// </summary>
public class DidException : Exception
{
    /// <summary>
    /// Initializes a new instance of the DidException class
    /// </summary>
    public DidException() : base()
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the DidException class with a specified error message
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public DidException(string message) : base(message)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the DidException class with a specified error message and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public DidException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
