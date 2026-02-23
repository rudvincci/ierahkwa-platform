using System.Collections.ObjectModel;
using Minio;
using Minio.DataModel;
using Moq;

namespace Mamey.Persistence.Minio.Tests.Fixtures;

/// <summary>
/// Factory for creating mock Minio clients for unit tests.
/// </summary>
public static class MockMinioClientFactory
{
    /// <summary>
    /// Creates a mock Minio client with default behaviors.
    /// </summary>
    /// <returns>A configured mock Minio client.</returns>
    public static Mock<IMinioClient> CreateDefault()
    {
        var mock = new Mock<IMinioClient>();
        
        // Setup default successful responses
        mock.Setup(x => x.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
            
        mock.Setup(x => x.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
            
        mock.Setup(x => x.RemoveBucketAsync(It.IsAny<RemoveBucketArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
            
        mock.Setup(x => x.ListBucketsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ListAllMyBucketsResult { Buckets = new Collection<Bucket>() });
            
        // Commented out - Expression tree cannot contain optional arguments
        // mock.Setup(x => x.ListObjectsAsync(It.IsAny<ListObjectsArgs>()))
        //     .Returns(new Mock<IObservable<Item>>().Object);
            
        mock.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutObjectResponse(System.Net.HttpStatusCode.OK, "etag", null, 0, "objectName"));
            
        // Commented out - GetObjectAsync signature mismatch (returns Task, not Task<ObjectStat>)
        // This causes issues with mocking. Tests using GetObjectAsync will need integration tests.
        // mock.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectArgs>(), It.IsAny<CancellationToken>()))
        //     .Returns(Task.CompletedTask);
            
        mock.Setup(x => x.RemoveObjectAsync(It.IsAny<RemoveObjectArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
            
        // Commented out - ObjectStat is immutable and cannot be easily created/mocked
        // Tests using StatObjectAsync will need to be commented out or use integration tests
        // ObjectStat? objectStat = null;
        // try
        // {
        //     var constructor = typeof(ObjectStat).GetConstructor(
        //         new[] { typeof(string), typeof(string), typeof(long), typeof(DateTime), typeof(string), typeof(string) });
        //     if (constructor != null)
        //     {
        //         objectStat = constructor.Invoke(new object[] 
        //         { 
        //             "test-bucket", "test-object", 1024L, DateTime.UtcNow, "test-etag", "application/octet-stream" 
        //         }) as ObjectStat;
        //     }
        // }
        // catch
        // {
        //     // If constructor doesn't exist or fails, we'll need to handle this differently
        // }
        // 
        // if (objectStat != null)
        // {
        //     mock.Setup(x => x.StatObjectAsync(It.IsAny<StatObjectArgs>(), It.IsAny<CancellationToken>()))
        //         .ReturnsAsync(objectStat);
        // }
            
        mock.Setup(x => x.PresignedGetObjectAsync(It.IsAny<PresignedGetObjectArgs>()))
            .ReturnsAsync("https://example.com/presigned-url");
            
        mock.Setup(x => x.PresignedPutObjectAsync(It.IsAny<PresignedPutObjectArgs>()))
            .ReturnsAsync("https://example.com/presigned-url");
            
        return mock;
    }
    
    /// <summary>
    /// Creates a mock Minio client that throws exceptions for testing error handling.
    /// </summary>
    /// <param name="exception">The exception to throw.</param>
    /// <returns>A configured mock Minio client.</returns>
    public static Mock<IMinioClient> CreateWithException(Exception exception)
    {
        var mock = new Mock<IMinioClient>();
        
        mock.Setup(x => x.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
            
        mock.Setup(x => x.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
            
        mock.Setup(x => x.RemoveBucketAsync(It.IsAny<RemoveBucketArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
            
        mock.Setup(x => x.ListBucketsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
            
        // Commented out - Expression tree cannot contain optional arguments
        // mock.Setup(x => x.ListObjectsAsync(It.IsAny<ListObjectsArgs>()))
        //     .Throws(exception);
            
        mock.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
            
        mock.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
            
        mock.Setup(x => x.RemoveObjectAsync(It.IsAny<RemoveObjectArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
            
        mock.Setup(x => x.StatObjectAsync(It.IsAny<StatObjectArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
            
        mock.Setup(x => x.PresignedGetObjectAsync(It.IsAny<PresignedGetObjectArgs>()))
            .ThrowsAsync(exception);
            
        mock.Setup(x => x.PresignedPutObjectAsync(It.IsAny<PresignedPutObjectArgs>()))
            .ThrowsAsync(exception);
            
        return mock;
    }
    
    /// <summary>
    /// Creates a mock Minio client that simulates bucket not found scenarios.
    /// </summary>
    /// <returns>A configured mock Minio client.</returns>
    public static Mock<IMinioClient> CreateBucketNotFound()
    {
        var mock = new Mock<IMinioClient>();
        
        mock.Setup(x => x.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
            
        mock.Setup(x => x.ListObjectsAsync(It.IsAny<ListObjectsArgs>()))
            .Throws(new Minio.Exceptions.BucketNotFoundException("Bucket not found"));
            
        mock.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Minio.Exceptions.BucketNotFoundException("Bucket not found"));
            
        mock.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Minio.Exceptions.BucketNotFoundException("Bucket not found"));
            
        return mock;
    }
    
    /// <summary>
    /// Creates a mock Minio client that simulates object not found scenarios.
    /// </summary>
    /// <returns>A configured mock Minio client.</returns>
    public static Mock<IMinioClient> CreateObjectNotFound()
    {
        var mock = new Mock<IMinioClient>();
        
        mock.Setup(x => x.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
            
        mock.Setup(x => x.StatObjectAsync(It.IsAny<StatObjectArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Minio.Exceptions.ObjectNotFoundException("Object not found", new Exception()));
            
        mock.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Minio.Exceptions.ObjectNotFoundException("Object not found", new Exception()));
            
        return mock;
    }
}

