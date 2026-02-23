using Mamey.Persistence.Minio;
using Mamey.Persistence.Minio.Builders;
using Mamey.Persistence.Minio.Models.DTOs;

namespace Mamey.Persistence.Minio.Examples;

/// <summary>
/// Examples demonstrating the fluent builder pattern for Minio operations.
/// </summary>
public static class MinioBuilderExamples
{
    /// <summary>
    /// Example of uploading a file with various configurations.
    /// </summary>
    public static async Task UploadFileExample(IObjectService objectService)
    {
        // Simple file upload
        using var fileStream = File.OpenRead("document.pdf");
        var request = "my-bucket"
            .UploadObject("documents/document.pdf", fileStream)
            .WithContentTypeFromFile("document.pdf")
            .WithMetadata("author", "John Doe")
            .WithMetadata("department", "Engineering")
            .WithTags(new Dictionary<string, string> { { "project", "alpha" }, { "status", "draft" } })
            .WithCacheControl("max-age=3600")
            .WithEncryption("AES256")
            .Build();

        var metadata = await objectService.UploadAsync(request);

        Console.WriteLine($"Uploaded file with ETag: {metadata.ETag}");
    }

    /// <summary>
    /// Example of uploading a large file with multipart upload.
    /// </summary>
    public static async Task UploadLargeFileExample(IObjectService objectService)
    {
        using var largeFileStream = File.OpenRead("large-video.mp4");
        
        var progress = new Progress<MultipartUploadProgress>(p =>
        {
            Console.WriteLine($"Upload progress: {p.Percentage:F1}% ({p.BytesUploaded:N0}/{p.TotalSize:N0} bytes)");
            Console.WriteLine($"Speed: {p.BytesPerSecond:N0} bytes/sec, ETA: {p.EstimatedTimeRemaining}");
        });

        var request = "my-bucket"
            .UploadObject("videos/large-video.mp4", largeFileStream)
            .WithContentTypeFromFile("large-video.mp4")
            .WithMultipartUpload(partSize: 10 * 1024 * 1024, maxConcurrency: 6) // 10MB parts, 6 concurrent
            .WithMetadata("duration", "120:30")
            .WithMetadata("resolution", "4K")
            .WithTags(new Dictionary<string, string> { { "type", "video" }, { "quality", "4k" } })
            .BuildMultipart();

        var metadata = await objectService.UploadMultipartAsync(request, progress);

        Console.WriteLine($"Uploaded large file with ETag: {metadata.ETag}");
    }

    /// <summary>
    /// Example of uploading bytes with encryption.
    /// </summary>
    public static async Task UploadBytesExample(IObjectService objectService)
    {
        var data = System.Text.Encoding.UTF8.GetBytes("Hello, Minio!");
        
        var request = "my-bucket"
            .UploadBytes("text/hello.txt", data)
            .WithContentType("text/plain")
            .WithEncryption("AES256", "my-key-id")
            .WithMetadata("created-by", "system")
            .WithCacheControl("no-cache")
            .Build();

        var metadata = await objectService.UploadAsync(request);

        Console.WriteLine($"Uploaded text with ETag: {metadata.ETag}");
    }

    /// <summary>
    /// Example of downloading with range requests and conditional headers.
    /// </summary>
    public static async Task DownloadWithRangeExample(IObjectService objectService)
    {
        using var outputStream = new MemoryStream();
        
        var progress = new Progress<long>(bytesDownloaded =>
        {
            Console.WriteLine($"Downloaded: {bytesDownloaded:N0} bytes");
        });

        var config = "my-bucket"
            .DownloadObject("documents/large-document.pdf")
            .WithRange(1024, 2048) // Download bytes 1024-2048
            .IfModifiedSince(DateTime.UtcNow.AddDays(-1))
            .WithResponseContentType("application/pdf")
            .WithResponseContentDisposition("attachment; filename=\"document.pdf\"")
            .Build();

        await objectService.DownloadAsync(config, outputStream, progress);

        Console.WriteLine($"Downloaded {outputStream.Length} bytes");
    }

    /// <summary>
    /// Example of downloading a specific version.
    /// </summary>
    public static async Task DownloadVersionExample(IObjectService objectService)
    {
        using var outputStream = new MemoryStream();
        
        var config = "my-bucket"
            .DownloadObject("documents/document.pdf")
            .WithVersion("version-id-123")
            .IfMatch("etag-value")
            .Build();

        await objectService.DownloadAsync(config, outputStream);

        Console.WriteLine($"Downloaded version with {outputStream.Length} bytes");
    }

    /// <summary>
    /// Example of configuring a bucket with comprehensive settings.
    /// </summary>
    public static async Task ConfigureBucketExample(IBucketService bucketService, ILifecycleService lifecycleService, IBucketPolicyService policyService)
    {
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
            .WithObjectLock(enabled: true, ObjectRetentionMode.Compliance, defaultRetentionDays: 2555) // 7 years
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

        // Apply bucket configuration
        if (configuration.VersioningEnabled)
        {
            await bucketService.EnableVersioningAsync(configuration.BucketName);
        }

        if (configuration.Tags != null)
        {
            await bucketService.SetBucketTagsAsync(new Models.Requests.SetBucketTagsRequest
            {
                BucketName = configuration.BucketName,
                Tags = configuration.Tags
            });
        }

        if (configuration.LifecycleRules != null)
        {
            await lifecycleService.SetLifecycleConfigurationAsync(new Models.Requests.SetLifecycleConfigurationRequest
            {
                BucketName = configuration.BucketName,
                Configuration = new LifecycleConfiguration { Rules = configuration.LifecycleRules }
            });
        }

        if (!string.IsNullOrEmpty(configuration.PolicyJson))
        {
            await policyService.SetBucketPolicyAsync(configuration.BucketName, configuration.PolicyJson);
        }

        Console.WriteLine($"Configured bucket: {configuration.BucketName}");
    }

    /// <summary>
    /// Example of uploading with progress reporting and error handling.
    /// </summary>
    public static async Task UploadWithProgressExample(IObjectService objectService)
    {
        try
        {
            using var fileStream = File.OpenRead("large-file.zip");
            
            var progress = new Progress<MultipartUploadProgress>(p =>
            {
                Console.WriteLine($"Upload: {p.Percentage:F1}% | " +
                                $"Parts: {p.CompletedParts}/{p.TotalParts} | " +
                                $"Speed: {p.BytesPerSecond:N0} B/s | " +
                                $"ETA: {p.EstimatedTimeRemaining?.ToString(@"hh\:mm\:ss") ?? "Unknown"}");
            });

            var request = "my-bucket"
                .UploadObject("backups/large-file.zip", fileStream)
                .WithContentTypeFromFile("large-file.zip")
                .WithMultipartUpload(partSize: 5 * 1024 * 1024, maxConcurrency: 4)
                .WithMetadata("backup-date", DateTime.UtcNow.ToString("yyyy-MM-dd"))
                .WithTags(new Dictionary<string, string> { { "type", "backup" }, { "priority", "high" } })
                .WithEncryption("AES256")
                .BuildMultipart();

            var metadata = await objectService.UploadMultipartAsync(request, progress);

            Console.WriteLine($"Upload completed successfully. ETag: {metadata.ETag}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Upload failed: {ex.Message}");
        }
    }
}
