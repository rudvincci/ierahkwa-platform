# Mamey.Persistence.Minio

A robust, production-ready Minio client library for .NET that follows Mamey framework conventions and provides comprehensive object storage capabilities.

## Features

### Core Capabilities
- **Complete Object Storage**: Upload, download, copy, delete objects with full metadata support
- **Bucket Management**: Create, delete, list buckets with versioning, tagging, and encryption
- **Presigned URLs**: Generate secure temporary access URLs for objects
- **Multipart Uploads**: Efficient handling of large files with parallel uploads and progress reporting
- **Stream Operations**: Both developer-managed streams and convenience methods for files and bytes

### Resilience & Reliability
- **Retry Policies**: Exponential backoff with jitter for transient failures
- **Circuit Breaker**: Automatic failure detection and recovery
- **Comprehensive Error Handling**: Detailed exception hierarchy with error codes and context
- **Structured Logging**: Full observability with correlation tracking

### Developer Experience
- **Fluent Builder Pattern**: Intuitive configuration for complex operations
- **Domain-Separated Services**: Clean architecture with focused responsibilities
- **Mamey Framework Integration**: Follows all Mamey conventions and patterns
- **Type Safety**: Full compile-time type safety for all operations

### Advanced Features
- **Object Versioning**: Enable/disable versioning, manage object versions
- **Lifecycle Management**: Automatic transitions and expiration rules
- **Object Lock & Retention**: Legal hold and compliance features
- **Bucket Policies**: Fine-grained access control and notifications
- **Encryption Support**: Server-side encryption with multiple algorithms

## Quick Start

### Installation

```bash
dotnet add package Mamey.Persistence.Minio
```

### Configuration

```csharp
using Mamey.Persistence.Minio;

// In Program.cs or Startup.cs
builder.AddMamey(services =>
{
    services.AddMinio(options =>
    {
        options.Endpoint = "localhost:9000";
        options.AccessKey = "minioadmin";
        options.SecretKey = "minioadmin";
        options.UseSSL = false;
        
        // Configure resilience
        options.RetryPolicy.MaxRetries = 3;
        options.RetryPolicy.InitialDelay = TimeSpan.FromSeconds(1);
        options.RetryPolicy.BackoffMultiplier = 2.0;
        
        // Configure circuit breaker
        options.CircuitBreaker = new CircuitBreakerPolicy
        {
            FailureThreshold = 5,
            DurationOfBreak = TimeSpan.FromSeconds(30)
        };
    });
});
```

### Basic Usage

```csharp
// Inject services
public class MyService
{
    private readonly IBucketService _bucketService;
    private readonly IObjectService _objectService;
    
    public MyService(IBucketService bucketService, IObjectService objectService)
    {
        _bucketService = bucketService;
        _objectService = objectService;
    }
    
    public async Task UploadFile()
    {
        // Simple file upload
        var metadata = await _objectService.UploadFileAsync(
            "my-bucket", 
            "documents/file.pdf", 
            "/path/to/file.pdf"
        );
        
        Console.WriteLine($"Uploaded with ETag: {metadata.ETag}");
    }
}
```

## Fluent Builder Pattern

The library provides a powerful fluent builder pattern for complex operations:

### Upload Operations

```csharp
// Upload with comprehensive configuration
var metadata = await "my-bucket"
    .UploadObject("documents/report.pdf", fileStream)
    .WithContentTypeFromFile("report.pdf")
    .WithMetadata("author", "John Doe")
    .WithMetadata("department", "Engineering")
    .WithTags(new Dictionary<string, string> 
    { 
        { "project", "alpha" }, 
        { "status", "draft" } 
    })
    .WithCacheControl("max-age=3600")
    .WithEncryption("AES256")
    .Build()
    .UploadAsync(_objectService);

// Large file with multipart upload
var progress = new Progress<MultipartUploadProgress>(p =>
{
    Console.WriteLine($"Progress: {p.Percentage:F1}% ({p.BytesUploaded:N0}/{p.TotalSize:N0} bytes)");
});

var metadata = await "my-bucket"
    .UploadObject("videos/large-video.mp4", videoStream)
    .WithContentTypeFromFile("large-video.mp4")
    .WithMultipartUpload(partSize: 10 * 1024 * 1024, maxConcurrency: 6)
    .WithMetadata("duration", "120:30")
    .WithTags(new Dictionary<string, string> { { "type", "video" } })
    .BuildMultipart()
    .UploadMultipartAsync(_objectService, progress);
```

### Download Operations

```csharp
// Download with range requests and conditional headers
using var outputStream = new MemoryStream();

var progress = new Progress<long>(bytesDownloaded =>
{
    Console.WriteLine($"Downloaded: {bytesDownloaded:N0} bytes");
});

await "my-bucket"
    .DownloadObject("documents/large-document.pdf")
    .WithRange(1024, 2048) // Download bytes 1024-2048
    .IfModifiedSince(DateTime.UtcNow.AddDays(-1))
    .WithResponseContentType("application/pdf")
    .WithResponseContentDisposition("attachment; filename=\"document.pdf\"")
    .Build()
    .DownloadAsync(_objectService, outputStream, progress);

// Download specific version
await "my-bucket"
    .DownloadObject("documents/document.pdf")
    .WithVersion("version-id-123")
    .IfMatch("etag-value")
    .Build()
    .DownloadAsync(_objectService, outputStream);
```

### Bucket Configuration

```csharp
// Configure bucket with comprehensive settings
var configuration = "my-bucket"
    .ConfigureBucket()
    .WithVersioning(mfaDeleteEnabled: true)
    .WithTags(new Dictionary<string, string>
    {
        { "environment", "production" },
        { "team", "engineering" },
        { "cost-center", "IT-001" }
    })
    .WithExpirationRule("delete-old-logs", "logs/", expirationDays: 30)
    .WithTransitionRule("archive-documents", "documents/", transitionDays: 90, "GLACIER")
    .WithObjectLock(enabled: true, ObjectRetentionMode.Compliance, defaultRetentionDays: 2555)
    .WithEncryption("AES256", "my-kms-key-id")
    .WithPolicy("""
        {
            "Version": "2012-10-17",
            "Statement": [
                {
                    "Effect": "Allow",
                    "Principal": "*",
                    "Action": "s3:GetObject",
                    "Resource": "arn:aws:s3:::my-bucket/*"
                }
            ]
        }
        """)
    .Build();

// Apply configuration
if (configuration.VersioningEnabled)
{
    await _bucketService.EnableVersioningAsync(configuration.BucketName);
}

if (configuration.Tags != null)
{
    await _bucketService.SetBucketTagsAsync(new SetBucketTagsRequest
    {
        BucketName = configuration.BucketName,
        Tags = configuration.Tags
    });
}
```

## Service Architecture

The library is organized into domain-separated services:

### Core Services

- **`IBucketService`**: Bucket management, versioning, tagging, encryption
- **`IObjectService`**: Object operations, multipart uploads, file helpers
- **`IPresignedUrlService`**: Presigned URL generation for secure access

### Specialized Services

- **`IBucketPolicyService`**: Bucket policies and notifications
- **`ILifecycleService`**: Lifecycle configuration and rules
- **`IObjectLockService`**: Object lock and retention policies

### Example Usage

```csharp
public class DocumentService
{
    private readonly IObjectService _objectService;
    private readonly IBucketService _bucketService;
    private readonly ILifecycleService _lifecycleService;
    
    public DocumentService(
        IObjectService objectService, 
        IBucketService bucketService,
        ILifecycleService lifecycleService)
    {
        _objectService = objectService;
        _bucketService = bucketService;
        _lifecycleService = lifecycleService;
    }
    
    public async Task StoreDocumentAsync(string documentPath, string bucketName)
    {
        // Ensure bucket exists
        if (!await _bucketService.BucketExistsAsync(bucketName))
        {
            await _bucketService.CreateBucketAsync(bucketName);
            await _bucketService.EnableVersioningAsync(bucketName);
        }
        
        // Upload document with metadata
        var metadata = await _objectService.UploadFileAsync(
            bucketName, 
            $"documents/{Path.GetFileName(documentPath)}", 
            documentPath,
            metadata: new Dictionary<string, string>
            {
                { "uploaded-by", Environment.UserName },
                { "upload-date", DateTime.UtcNow.ToString("yyyy-MM-dd") }
            }
        );
        
        // Set up lifecycle rule for document cleanup
        await _lifecycleService.SetLifecycleConfigurationAsync(new SetLifecycleConfigurationRequest
        {
            BucketName = bucketName,
            Configuration = new LifecycleConfiguration
            {
                Rules = new List<LifecycleRule>
                {
                    new()
                    {
                        Id = "delete-old-documents",
                        Status = LifecycleRuleStatus.Enabled,
                        Filter = new LifecycleFilter { Prefix = "documents/" },
                        Expiration = new LifecycleExpiration { Days = 365 }
                    }
                }
            }
        });
    }
}
```

## Convenience Methods

The library provides convenient methods for common operations:

```csharp
// File operations
var metadata = await _objectService.UploadFileAsync("bucket", "file.pdf", "/path/to/file.pdf");
await _objectService.DownloadToFileAsync("bucket", "file.pdf", "/path/to/output.pdf");

// Byte array operations
var data = System.Text.Encoding.UTF8.GetBytes("Hello, Minio!");
var metadata = await _objectService.UploadBytesAsync("bucket", "text.txt", data);
var downloadedData = await _objectService.DownloadAsBytesAsync("bucket", "text.txt");

// Stream operations with progress
var progress = new Progress<long>(bytes => Console.WriteLine($"Downloaded: {bytes} bytes"));
await _objectService.DownloadAsync("bucket", "file.pdf", outputStream, progress);
```

## Error Handling

The library provides a comprehensive exception hierarchy:

```csharp
try
{
    await _objectService.UploadFileAsync("bucket", "file.pdf", "/path/to/file.pdf");
}
catch (BucketNotFoundException ex)
{
    // Handle bucket not found
    Console.WriteLine($"Bucket not found: {ex.BucketName}");
}
catch (ObjectNotFoundException ex)
{
    // Handle object not found
    Console.WriteLine($"Object not found: {ex.ObjectName}");
}
catch (RetryExhaustedException ex)
{
    // Handle retry exhaustion
    Console.WriteLine($"Operation failed after {ex.AttemptCount} attempts");
}
catch (CircuitBreakerOpenException ex)
{
    // Handle circuit breaker open
    Console.WriteLine($"Circuit breaker is open: {ex.Reason}");
}
catch (MinioServiceException ex)
{
    // Handle general Minio service errors
    Console.WriteLine($"Minio error: {ex.Code} - {ex.Reason}");
}
```

## Configuration Options

```csharp
public class MinioOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool UseSSL { get; set; } = false;
    public string? Region { get; set; }
    public string? SessionToken { get; set; }
    
    // Resilience configuration
    public RetryPolicy RetryPolicy { get; set; } = new();
    public CircuitBreakerPolicy? CircuitBreaker { get; set; }
}

public class RetryPolicy
{
    public int MaxRetries { get; set; } = 3;
    public TimeSpan InitialDelay { get; set; } = TimeSpan.FromSeconds(1);
    public double BackoffMultiplier { get; set; } = 2.0;
    public bool UseJitter { get; set; } = true;
}

public class CircuitBreakerPolicy
{
    public int FailureThreshold { get; set; } = 5;
    public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);
    public int SamplingDuration { get; set; } = 60;
}
```

## Logging

The library uses structured logging throughout:

```csharp
// Configure logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// Logs include correlation IDs and structured data
// Example log entries:
// [INFO] Uploading object documents/report.pdf to bucket my-bucket
// [DEBUG] Using part size 5242880 for 15 parts
// [INFO] Successfully uploaded object documents/report.pdf with ETag "abc123"
```

## Performance Considerations

- **Multipart Uploads**: Automatically used for files > 100MB
- **Parallel Processing**: Configurable concurrency for multipart uploads
- **Streaming**: Efficient memory usage with streaming operations
- **Connection Pooling**: Reuses HTTP connections for better performance
- **Retry Policies**: Prevents unnecessary retries with exponential backoff

## Testing

The library is designed for easy testing with dependency injection:

```csharp
// Unit test example
[Test]
public async Task UploadFile_ShouldReturnMetadata()
{
    // Arrange
    var mockObjectService = new Mock<IObjectService>();
    var expectedMetadata = new ObjectMetadata { ETag = "abc123", Size = 1024 };
    mockObjectService.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(expectedMetadata);
    
    var service = new MyService(mockObjectService.Object, null);
    
    // Act
    var result = await service.UploadFile();
    
    // Assert
    Assert.That(result.ETag, Is.EqualTo("abc123"));
}
```

## License

This library is part of the Mamey framework and follows the same licensing terms.

## Contributing

Contributions are welcome! Please follow the Mamey framework conventions and ensure all tests pass.

## Support

For support and questions, please refer to the Mamey framework documentation or create an issue in the repository.
