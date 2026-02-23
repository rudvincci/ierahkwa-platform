using Mamey.Exceptions;

namespace Mamey.Persistence.Minio.Exceptions;

/// <summary>
/// Exception thrown when an object is not found.
/// </summary>
public class ObjectNotFoundException : MinioServiceException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectNotFoundException"/> class.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object that was not found.</param>
    public ObjectNotFoundException(string bucketName, string objectName)
        : base($"Object '{objectName}' not found in bucket '{bucketName}'", "object_not_found", 
               $"The object '{objectName}' does not exist in bucket '{bucketName}'")
    {
        BucketName = bucketName;
        ObjectName = objectName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectNotFoundException"/> class.
    /// </summary>
    /// <param name="bucketName">The name of the bucket.</param>
    /// <param name="objectName">The name of the object that was not found.</param>
    /// <param name="innerException">The inner exception.</param>
    public ObjectNotFoundException(string bucketName, string objectName, Exception? innerException)
        : base($"Object '{objectName}' not found in bucket '{bucketName}'", "object_not_found", 
               $"The object '{objectName}' does not exist in bucket '{bucketName}'", innerException)
    {
        BucketName = bucketName;
        ObjectName = objectName;
    }

    /// <summary>
    /// Gets the name of the bucket.
    /// </summary>
    public string BucketName { get; }

    /// <summary>
    /// Gets the name of the object that was not found.
    /// </summary>
    public string ObjectName { get; }
}
