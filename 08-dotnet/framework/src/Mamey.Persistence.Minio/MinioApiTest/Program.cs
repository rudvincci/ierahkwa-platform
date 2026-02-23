using Minio;
using Minio.DataModel;
using Minio.Exceptions;

// Test basic Minio client functionality
var client = new MinioClient()
    .WithEndpoint("localhost:9000")
    .WithCredentials("minioadmin", "minioadmin")
    .WithSSL(false)
    .Build();

try
{
    // Test basic operations
    Console.WriteLine("Testing Minio client...");
    
    // List buckets
    var buckets = await client.ListBucketsAsync();
    Console.WriteLine($"Found {buckets.Buckets.Count} buckets");
    
    // Test bucket operations
    var bucketName = "test-bucket-" + Guid.NewGuid().ToString("N")[..8];
    Console.WriteLine($"Creating test bucket: {bucketName}");
    
    await client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
    Console.WriteLine("Bucket created successfully");
    
    // Test object operations
    var objectName = "test-object.txt";
    var content = "Hello, Minio!";
    var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);
    
    using var stream = new MemoryStream(contentBytes);
    await client.PutObjectAsync(new PutObjectArgs()
        .WithBucket(bucketName)
        .WithObject(objectName)
        .WithStreamData(stream)
        .WithObjectSize(contentBytes.Length));
    Console.WriteLine("Object uploaded successfully");
    
    // Test object stat
    var stat = await client.StatObjectAsync(new StatObjectArgs()
        .WithBucket(bucketName)
        .WithObject(objectName));
    Console.WriteLine($"Object size: {stat.Size}, Last modified: {stat.LastModified}");
    
    // Test presigned URL
    var presignedUrl = await client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
        .WithBucket(bucketName)
        .WithObject(objectName)
        .WithExpiry(3600));
    Console.WriteLine($"Presigned URL: {presignedUrl}");
    
    // Clean up
    await client.RemoveObjectAsync(new RemoveObjectArgs()
        .WithBucket(bucketName)
        .WithObject(objectName));
    await client.RemoveBucketAsync(new RemoveBucketArgs().WithBucket(bucketName));
    Console.WriteLine("Cleanup completed");
    
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