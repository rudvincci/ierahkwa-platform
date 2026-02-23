using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;

namespace Mamey.Persistence.Minio.Tests.Helpers;

/// <summary>
/// Helper class for generating test data.
/// </summary>
public static class TestDataGenerator
{
    /// <summary>
    /// Creates a sample bucket name for testing.
    /// </summary>
    /// <param name="suffix">Optional suffix to make the name unique.</param>
    /// <returns>A test bucket name.</returns>
    public static string CreateBucketName(string? suffix = null)
    {
        var baseName = "test-bucket";
        return string.IsNullOrEmpty(suffix) ? baseName : $"{baseName}-{suffix}";
    }
    
    /// <summary>
    /// Creates a sample object name for testing.
    /// </summary>
    /// <param name="suffix">Optional suffix to make the name unique.</param>
    /// <returns>A test object name.</returns>
    public static string CreateObjectName(string? suffix = null)
    {
        var baseName = "test-object.txt";
        return string.IsNullOrEmpty(suffix) ? baseName : $"test-object-{suffix}.txt";
    }
    
    /// <summary>
    /// Creates sample test data as bytes.
    /// </summary>
    /// <param name="size">The size of the data in bytes.</param>
    /// <returns>Test data bytes.</returns>
    public static byte[] CreateTestData(int size = 1024)
    {
        var data = new byte[size];
        var random = new Random(42); // Fixed seed for reproducible tests
        random.NextBytes(data);
        return data;
    }
    
    /// <summary>
    /// Creates a sample stream with test data.
    /// </summary>
    /// <param name="size">The size of the data in bytes.</param>
    /// <returns>A memory stream with test data.</returns>
    public static MemoryStream CreateTestStream(int size = 1024)
    {
        var data = CreateTestData(size);
        return new MemoryStream(data);
    }
    
    /// <summary>
    /// Creates sample metadata for testing.
    /// </summary>
    /// <returns>A dictionary with sample metadata.</returns>
    public static Dictionary<string, string> CreateSampleMetadata()
    {
        return new Dictionary<string, string>
        {
            { "author", "Test Author" },
            { "department", "Engineering" },
            { "project", "Test Project" },
            { "version", "1.0.0" }
        };
    }
    
    /// <summary>
    /// Creates a sample PutObjectRequest for testing.
    /// </summary>
    /// <param name="bucketName">The bucket name.</param>
    /// <param name="objectName">The object name.</param>
    /// <param name="dataSize">The size of the test data.</param>
    /// <returns>A configured PutObjectRequest.</returns>
    public static PutObjectRequest CreatePutObjectRequest(string bucketName, string objectName, int dataSize = 1024)
    {
        return new PutObjectRequest
        {
            BucketName = bucketName,
            ObjectName = objectName,
            Data = CreateTestStream(dataSize),
            Size = dataSize,
            ContentType = "text/plain",
            Metadata = CreateSampleMetadata()
        };
    }
    
    /// <summary>
    /// Creates a sample MultipartUploadRequest for testing.
    /// </summary>
    /// <param name="bucketName">The bucket name.</param>
    /// <param name="objectName">The object name.</param>
    /// <param name="dataSize">The size of the test data.</param>
    /// <returns>A configured MultipartUploadRequest.</returns>
    public static MultipartUploadRequest CreateMultipartUploadRequest(string bucketName, string objectName, int dataSize = 1024)
    {
        return new MultipartUploadRequest
        {
            BucketName = bucketName,
            ObjectName = objectName,
            Stream = CreateTestStream(dataSize),
            ContentType = "text/plain",
            Metadata = CreateSampleMetadata(),
            PartSize = 5 * 1024 * 1024, // 5MB
            MaxConcurrency = 4
        };
    }
    
    /// <summary>
    /// Creates a sample ObjectMetadata for testing.
    /// </summary>
    /// <param name="objectName">The object name.</param>
    /// <param name="size">The object size.</param>
    /// <returns>A configured ObjectMetadata.</returns>
    public static ObjectMetadata CreateObjectMetadata(string objectName, long size = 1024)
    {
        return new ObjectMetadata
        {
            Name = objectName,
            Size = size,
            LastModified = DateTime.UtcNow,
            ETag = "test-etag-123",
            ContentType = "text/plain"
        };
    }
    
    /// <summary>
    /// Creates a sample BucketInfo for testing.
    /// </summary>
    /// <param name="bucketName">The bucket name.</param>
    /// <returns>A configured BucketInfo.</returns>
    public static BucketInfo CreateBucketInfo(string bucketName)
    {
        return new BucketInfo
        {
            Name = bucketName,
            CreationDate = DateTime.UtcNow.AddDays(-30)
        };
    }
    
    /// <summary>
    /// Creates sample bucket tags for testing.
    /// </summary>
    /// <returns>A dictionary with sample bucket tags.</returns>
    public static Dictionary<string, string> CreateSampleBucketTags()
    {
        return new Dictionary<string, string>
        {
            { "environment", "test" },
            { "team", "engineering" },
            { "cost-center", "IT-001" },
            { "project", "minio-library" }
        };
    }
    
    /// <summary>
    /// Creates a sample BucketVersioningInfo for testing.
    /// </summary>
    /// <param name="status">The versioning status.</param>
    /// <param name="mfaDelete">The MFA delete status.</param>
    /// <returns>A configured BucketVersioningInfo.</returns>
    public static BucketVersioningInfo CreateBucketVersioningInfo(VersioningStatus status = VersioningStatus.Enabled, MfaDeleteStatus mfaDelete = MfaDeleteStatus.Disabled)
    {
        return new BucketVersioningInfo
        {
            Status = status,
            MfaDelete = mfaDelete
        };
    }
    
    /// <summary>
    /// Creates a sample LifecycleConfiguration for testing.
    /// </summary>
    /// <returns>A configured LifecycleConfiguration.</returns>
    public static LifecycleConfiguration CreateLifecycleConfiguration()
    {
        return new LifecycleConfiguration
        {
            Rules = new List<LifecycleRule>
            {
                new()
                {
                    Id = "delete-old-logs",
                    Status = LifecycleRuleStatus.Enabled,
                    Filter = new LifecycleFilter { Prefix = "logs/" },
                    Expiration = new LifecycleExpiration { Days = 30 }
                },
                new()
                {
                    Id = "archive-documents",
                    Status = LifecycleRuleStatus.Enabled,
                    Filter = new LifecycleFilter { Prefix = "documents/" },
                    Transitions = new List<LifecycleTransition>
                    {
                        new() { Days = 90, StorageClass = "GLACIER" }
                    }
                }
            }
        };
    }
    
    /// <summary>
    /// Creates a sample ObjectLockConfiguration for testing.
    /// </summary>
    /// <returns>A configured ObjectLockConfiguration.</returns>
    public static ObjectLockConfiguration CreateObjectLockConfiguration()
    {
        return new ObjectLockConfiguration
        {
            ObjectLockEnabled = true,
            DefaultRetention = new DefaultRetention
            {
                Mode = ObjectRetentionMode.Compliance,
                Days = 2555 // 7 years
            }
        };
    }
}



