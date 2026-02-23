using Mamey.Exceptions;

namespace Mamey.Persistence.Minio.Exceptions;

/// <summary>
/// Exception thrown when retry attempts are exhausted.
/// </summary>
public class RetryExhaustedException : MinioServiceException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RetryExhaustedException"/> class.
    /// </summary>
    /// <param name="operation">The operation that failed after retries.</param>
    /// <param name="attempts">The number of retry attempts made.</param>
    /// <param name="innerException">The inner exception.</param>
    public RetryExhaustedException(string operation, int attempts, Exception? innerException = null)
        : base($"Operation '{operation}' failed after {attempts} retry attempts", "retry_exhausted", 
               $"The operation '{operation}' failed after {attempts} retry attempts", innerException)
    {
        Operation = operation;
        Attempts = attempts;
    }

    /// <summary>
    /// Gets the operation that failed.
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Gets the number of retry attempts made.
    /// </summary>
    public int Attempts { get; }
}
