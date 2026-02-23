namespace Mamey.Auth.Decentralized.Exceptions;

/// <summary>
/// Exception thrown when Verifiable Credential validation fails
/// </summary>
public class VcValidationException : VcException
{
    /// <summary>
    /// The validation error details
    /// </summary>
    public string? ValidationError { get; }
    
    /// <summary>
    /// The credential ID that failed validation
    /// </summary>
    public string? CredentialId { get; }
    
    /// <summary>
    /// Initializes a new instance of the VcValidationException class
    /// </summary>
    public VcValidationException() : base()
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the VcValidationException class with a specified error message
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public VcValidationException(string message) : base(message)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the VcValidationException class with a specified error message and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public VcValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the VcValidationException class with a specified validation error and error message
    /// </summary>
    /// <param name="validationError">The validation error details</param>
    /// <param name="message">The message that describes the error</param>
    public VcValidationException(string validationError, string message) : base(message)
    {
        ValidationError = validationError;
    }
    
    /// <summary>
    /// Initializes a new instance of the VcValidationException class with a specified credential ID, validation error, and error message
    /// </summary>
    /// <param name="credentialId">The credential ID that failed validation</param>
    /// <param name="validationError">The validation error details</param>
    /// <param name="message">The message that describes the error</param>
    public VcValidationException(string credentialId, string validationError, string message) : base(message)
    {
        CredentialId = credentialId;
        ValidationError = validationError;
    }
    
    /// <summary>
    /// Initializes a new instance of the VcValidationException class with a specified credential ID, validation error, error message, and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="credentialId">The credential ID that failed validation</param>
    /// <param name="validationError">The validation error details</param>
    /// <param name="message">The error message that explains the reason for the exception</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public VcValidationException(string credentialId, string validationError, string message, Exception innerException) : base(message, innerException)
    {
        CredentialId = credentialId;
        ValidationError = validationError;
    }
}
