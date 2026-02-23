namespace Mamey.Auth.Decentralized.Exceptions;

/// <summary>
/// Exception thrown when cryptographic operations fail
/// </summary>
public class CryptoException : Exception
{
    /// <summary>
    /// The cryptographic algorithm that failed
    /// </summary>
    public string? Algorithm { get; }
    
    /// <summary>
    /// The operation that failed
    /// </summary>
    public string? Operation { get; }
    
    /// <summary>
    /// Initializes a new instance of the CryptoException class
    /// </summary>
    public CryptoException() : base()
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the CryptoException class with a specified error message
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public CryptoException(string message) : base(message)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the CryptoException class with a specified error message and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public CryptoException(string message, Exception innerException) : base(message, innerException)
    {
    }
    
    /// <summary>
    /// Initializes a new instance of the CryptoException class with a specified algorithm and error message
    /// </summary>
    /// <param name="algorithm">The cryptographic algorithm that failed</param>
    /// <param name="message">The message that describes the error</param>
    public CryptoException(string algorithm, string message) : base(message)
    {
        Algorithm = algorithm;
    }
    
    /// <summary>
    /// Initializes a new instance of the CryptoException class with a specified algorithm, operation, and error message
    /// </summary>
    /// <param name="algorithm">The cryptographic algorithm that failed</param>
    /// <param name="operation">The operation that failed</param>
    /// <param name="message">The message that describes the error</param>
    public CryptoException(string algorithm, string operation, string message) : base(message)
    {
        Algorithm = algorithm;
        Operation = operation;
    }
    
    /// <summary>
    /// Initializes a new instance of the CryptoException class with a specified algorithm, operation, error message, and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="algorithm">The cryptographic algorithm that failed</param>
    /// <param name="operation">The operation that failed</param>
    /// <param name="message">The error message that explains the reason for the exception</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public CryptoException(string algorithm, string operation, string message, Exception innerException) : base(message, innerException)
    {
        Algorithm = algorithm;
        Operation = operation;
    }
}
