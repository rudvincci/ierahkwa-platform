namespace Mamey.Auth.Decentralized.Exceptions;

/// <summary>
/// Exception thrown when DID resolution fails
/// </summary>
public class DidResolutionException : DidException
{
    /// <summary>
    /// The DID that failed to resolve
    /// </summary>
    public string Did { get; }
    
    /// <summary>
    /// The error code from the resolution
    /// </summary>
    public string? ErrorCode { get; }
    
    /// <summary>
    /// Initializes a new instance of the DidResolutionException class
    /// </summary>
    public DidResolutionException() : base()
    {
        Did = string.Empty;
    }
    
    /// <summary>
    /// Initializes a new instance of the DidResolutionException class with a specified error message
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public DidResolutionException(string message) : base(message)
    {
        Did = string.Empty;
    }
    
    /// <summary>
    /// Initializes a new instance of the DidResolutionException class with a specified error message and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public DidResolutionException(string message, Exception innerException) : base(message, innerException)
    {
        Did = string.Empty;
    }
    
    /// <summary>
    /// Initializes a new instance of the DidResolutionException class with a specified DID and error message
    /// </summary>
    /// <param name="did">The DID that failed to resolve</param>
    /// <param name="message">The message that describes the error</param>
    public DidResolutionException(string did, string message) : base(message)
    {
        Did = did;
    }
    
    /// <summary>
    /// Initializes a new instance of the DidResolutionException class with a specified DID, error code, and error message
    /// </summary>
    /// <param name="did">The DID that failed to resolve</param>
    /// <param name="errorCode">The error code from the resolution</param>
    /// <param name="message">The message that describes the error</param>
    public DidResolutionException(string did, string errorCode, string message) : base(message)
    {
        Did = did;
        ErrorCode = errorCode;
    }
    
    /// <summary>
    /// Initializes a new instance of the DidResolutionException class with a specified DID, error code, error message, and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="did">The DID that failed to resolve</param>
    /// <param name="errorCode">The error code from the resolution</param>
    /// <param name="message">The error message that explains the reason for the exception</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public DidResolutionException(string did, string errorCode, string message, Exception innerException) : base(message, innerException)
    {
        Did = did;
        ErrorCode = errorCode;
    }
}
