using Mamey.Persistence.Minio.Builders;

namespace Mamey.Persistence.Minio;

/// <summary>
/// Extension methods for creating fluent builders.
/// </summary>
public static class MinioBuilderExtensions
{
    /// <summary>
    /// Creates a new object upload builder.
    /// </summary>
    /// <param name="bucketName">The bucket name.</param>
    /// <param name="objectName">The object name.</param>
    /// <param name="stream">The data stream.</param>
    /// <returns>The upload builder.</returns>
    public static ObjectUploadBuilder UploadObject(this string bucketName, string objectName, Stream stream)
    {
        return new ObjectUploadBuilder(bucketName, objectName, stream);
    }

    /// <summary>
    /// Creates a new object upload builder for a file.
    /// </summary>
    /// <param name="bucketName">The bucket name.</param>
    /// <param name="objectName">The object name.</param>
    /// <param name="filePath">The file path.</param>
    /// <returns>The upload builder.</returns>
    public static ObjectUploadBuilder UploadFile(this string bucketName, string objectName, string filePath)
    {
        var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return new ObjectUploadBuilder(bucketName, objectName, stream);
    }

    /// <summary>
    /// Creates a new object upload builder for bytes.
    /// </summary>
    /// <param name="bucketName">The bucket name.</param>
    /// <param name="objectName">The object name.</param>
    /// <param name="data">The data bytes.</param>
    /// <returns>The upload builder.</returns>
    public static ObjectUploadBuilder UploadBytes(this string bucketName, string objectName, byte[] data)
    {
        var stream = new MemoryStream(data);
        return new ObjectUploadBuilder(bucketName, objectName, stream);
    }

    /// <summary>
    /// Creates a new object download builder.
    /// </summary>
    /// <param name="bucketName">The bucket name.</param>
    /// <param name="objectName">The object name.</param>
    /// <returns>The download builder.</returns>
    public static ObjectDownloadBuilder DownloadObject(this string bucketName, string objectName)
    {
        return new ObjectDownloadBuilder(bucketName, objectName);
    }

    /// <summary>
    /// Creates a new bucket configuration builder.
    /// </summary>
    /// <param name="bucketName">The bucket name.</param>
    /// <returns>The configuration builder.</returns>
    public static BucketConfigurationBuilder ConfigureBucket(this string bucketName)
    {
        return new BucketConfigurationBuilder(bucketName);
    }
}
