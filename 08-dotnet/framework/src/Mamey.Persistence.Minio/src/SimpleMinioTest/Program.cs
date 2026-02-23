using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Mamey.Persistence.Minio;
using Mamey.Persistence.Minio.Models.Requests;

// Test the simple Minio service
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<SimpleMinioService>();

var options = Options.Create(new MinioOptions
{
    Endpoint = "localhost:9000",
    AccessKey = "minioadmin",
    SecretKey = "minioadmin",
    UseSSL = false
});

var client = new MinioClient()
    .WithEndpoint("localhost:9000")
    .WithCredentials("minioadmin", "minioadmin")
    .WithSSL(false)
    .Build();

var service = new SimpleMinioService(client, options, logger);

try
{
    Console.WriteLine("Testing SimpleMinioService...");
    
    // Test listing buckets
    var buckets = await service.ListBucketsAsync();
    Console.WriteLine($"Found {buckets.Count} buckets");
    
    // Test creating a bucket
    var bucketName = "test-simple-" + Guid.NewGuid().ToString("N")[..8];
    Console.WriteLine($"Creating bucket: {bucketName}");
    await service.MakeBucketAsync(bucketName);
    
    // Test checking if bucket exists
    var exists = await service.BucketExistsAsync(bucketName);
    Console.WriteLine($"Bucket exists: {exists}");
    
    // Test putting an object
    var objectName = "test-object.txt";
    var content = "Hello from SimpleMinioService!";
    var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);
    
    using var stream = new MemoryStream(contentBytes);
    var putRequest = new PutObjectRequest
    {
        BucketName = bucketName,
        ObjectName = objectName,
        Data = stream,
        Size = contentBytes.Length,
        ContentType = "text/plain"
    };
    
    await service.PutObjectAsync(putRequest);
    Console.WriteLine("Object uploaded successfully");
    
    // Test getting object metadata
    var metadata = await service.StatObjectAsync(bucketName, objectName);
    Console.WriteLine($"Object size: {metadata.Size}, Content type: {metadata.ContentType}");
    
    // Test presigned URL
    var presignedRequest = new PresignedUrlRequest
    {
        BucketName = bucketName,
        ObjectName = objectName,
        ExpiresInSeconds = 3600
    };
    
    var presignedUrl = await service.PresignedGetObjectAsync(presignedRequest);
    Console.WriteLine($"Presigned URL: {presignedUrl.Url}");
    
    // Test removing object
    await service.RemoveObjectAsync(bucketName, objectName);
    Console.WriteLine("Object removed successfully");
    
    // Test removing bucket
    await service.RemoveBucketAsync(bucketName);
    Console.WriteLine("Bucket removed successfully");
    
    Console.WriteLine("All tests passed!");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Type: {ex.GetType().Name}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Inner: {ex.InnerException.Message}");
    }
}