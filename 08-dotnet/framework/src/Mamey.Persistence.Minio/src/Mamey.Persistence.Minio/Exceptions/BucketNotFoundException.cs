using Mamey.Exceptions;

namespace Mamey.Persistence.Minio.Exceptions;

/// <summary>
/// Exception thrown when a bucket is not found.
/// </summary>
public class BucketNotFoundException : MinioServiceException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BucketNotFoundException"/> class.
    /// </summary>
    /// <param name="bucketName">The name of the bucket that was not found.</param>
    public BucketNotFoundException(string bucketName)
        : base($"Bucket '{bucketName}' not found", "bucket_not_found", $"The bucket '{bucketName}' does not exist")
    {
        BucketName = bucketName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BucketNotFoundException"/> class.
    /// </summary>
    /// <param name="bucketName">The name of the bucket that was not found.</param>
    /// <param name="innerException">The inner exception.</param>
    public BucketNotFoundException(string bucketName, Exception? innerException)
        : base($"Bucket '{bucketName}' not found", "bucket_not_found", $"The bucket '{bucketName}' does not exist", innerException)
    {
        BucketName = bucketName;
    }

    /// <summary>
    /// Gets the name of the bucket that was not found.
    /// </summary>
    public string BucketName { get; }
}
