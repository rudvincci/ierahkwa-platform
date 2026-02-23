namespace Mamey.Auth.Decentralized.Exceptions;

/// <summary>
/// Exception thrown when a DID Document is invalid
/// </summary>
public class InvalidDidDocumentException : DidException
{
    /// <summary>
    /// The DID that the invalid document belongs to
    /// </summary>
    public string Did { get; }
    
    /// <summary>
    /// The validation error details
    /// </summary>
    public string? ValidationError { get; }
    
    /// <summary>
    /// Initializes a new instance of the InvalidDidDocumentException class
    /// </summary>
    public InvalidDidDocumentException() : base()
    {
        Did = string.Empty;
    }
    
    /// <summary>
    /// Initializes a new instance of the InvalidDidDocumentException class with a specified error message
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public InvalidDidDocumentException(string message) : base(message)
    {
        Did = string.Empty;
    }
    
    /// <summary>
    /// Initializes a new instance of the InvalidDidDocumentException class with a specified error message and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public InvalidDidDocumentException(string message, Exception innerException) : base(message, innerException)
    {
        Did = string.Empty;
    }
    
    /// <summary>
    /// Initializes a new instance of the InvalidDidDocumentException class with a specified DID and error message
    /// </summary>
    /// <param name="did">The DID that the invalid document belongs to</param>
    /// <param name="message">The message that describes the error</param>
    public InvalidDidDocumentException(string did, string message) : base(message)
    {
        Did = did;
    }
    
    /// <summary>
    /// Initializes a new instance of the InvalidDidDocumentException class with a specified DID, validation error, and error message
    /// </summary>
    /// <param name="did">The DID that the invalid document belongs to</param>
    /// <param name="validationError">The validation error details</param>
    /// <param name="message">The message that describes the error</param>
    public InvalidDidDocumentException(string did, string validationError, string message) : base(message)
    {
        Did = did;
        ValidationError = validationError;
    }
    
    /// <summary>
    /// Initializes a new instance of the InvalidDidDocumentException class with a specified DID, validation error, error message, and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="did">The DID that the invalid document belongs to</param>
    /// <param name="validationError">The validation error details</param>
    /// <param name="message">The error message that explains the reason for the exception</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public InvalidDidDocumentException(string did, string validationError, string message, Exception innerException) : base(message, innerException)
    {
        Did = did;
        ValidationError = validationError;
    }
}
