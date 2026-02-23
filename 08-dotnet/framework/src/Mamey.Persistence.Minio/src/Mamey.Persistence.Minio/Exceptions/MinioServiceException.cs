using Mamey.Exceptions;

namespace Mamey.Persistence.Minio.Exceptions;

/// <summary>
/// Represents an exception that occurs during Minio service operations.
/// </summary>
public class MinioServiceException : MameyException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MinioServiceException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="description">The error description.</param>
    /// <param name="innerException">The inner exception.</param>
    public MinioServiceException(string message, string errorCode, string description, Exception? innerException = null)
        : base(message, errorCode, description, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinioServiceException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="innerException">The inner exception.</param>
    public MinioServiceException(string message, string errorCode, Exception? innerException = null)
        : base(message, errorCode, message, innerException)
    {
    }
}

/// <summary>
/// Represents an exception that occurs during bucket operations.
/// </summary>
public class BucketOperationException : MinioServiceException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BucketOperationException"/> class.
    /// </summary>
    /// <param name="bucketName">The bucket name.</param>
    /// <param name="operation">The operation being performed.</param>
    /// <param name="innerException">The inner exception.</param>
    public BucketOperationException(string bucketName, string operation, Exception? innerException = null)
        : base($"Failed to {operation} bucket '{bucketName}'", $"bucket_{operation.ToLowerInvariant()}_failed", 
               $"An error occurred while {operation.ToLowerInvariant()}ing bucket '{bucketName}'", innerException)
    {
        BucketName = bucketName;
        Operation = operation;
    }

    /// <summary>
    /// Gets the bucket name.
    /// </summary>
    public string BucketName { get; }

    /// <summary>
    /// Gets the operation being performed.
    /// </summary>
    public string Operation { get; }
}

/// <summary>
/// Represents an exception that occurs during object operations.
/// </summary>
public class ObjectOperationException : MinioServiceException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectOperationException"/> class.
    /// </summary>
    /// <param name="bucketName">The bucket name.</param>
    /// <param name="objectName">The object name.</param>
    /// <param name="operation">The operation being performed.</param>
    /// <param name="innerException">The inner exception.</param>
    public ObjectOperationException(string bucketName, string objectName, string operation, Exception? innerException = null)
        : base($"Failed to {operation} object '{objectName}' in bucket '{bucketName}'", $"object_{operation.ToLowerInvariant()}_failed",
               $"An error occurred while {operation.ToLowerInvariant()}ing object '{objectName}' in bucket '{bucketName}'", innerException)
    {
        BucketName = bucketName;
        ObjectName = objectName;
        Operation = operation;
    }

    /// <summary>
    /// Gets the bucket name.
    /// </summary>
    public string BucketName { get; }

    /// <summary>
    /// Gets the object name.
    /// </summary>
    public string ObjectName { get; }

    /// <summary>
    /// Gets the operation being performed.
    /// </summary>
    public string Operation { get; }
}
