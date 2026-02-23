using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Persistence.Minio;
using Mamey.Persistence.Minio.Builders;
using Mamey.Persistence.Minio.Models.Requests;
using Testcontainers.Minio;
using Xunit;

namespace Mamey.Persistence.Minio.Tests.Integration;

/// <summary>
/// Integration tests for object operations using TestContainers.
/// </summary>
public class ObjectIntegrationTests : IClassFixture<MinioContainerFixture>
{
    private readonly MinioContainerFixture _fixture;
    private readonly IBucketService _bucketService;
    private readonly IObjectService _objectService;
    private readonly IPresignedUrlService _presignedUrlService;

    public ObjectIntegrationTests(MinioContainerFixture fixture)
    {
        _fixture = fixture;
        _bucketService = _fixture.ServiceProvider.GetRequiredService<IBucketService>();
        _objectService = _fixture.ServiceProvider.GetRequiredService<IObjectService>();
        _presignedUrlService = _fixture.ServiceProvider.GetRequiredService<IPresignedUrlService>();
    }

    [Fact]
    public async Task UploadAndDownloadObject_ShouldSucceed()
    {
        // Arrange
        var bucketName = $"test-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";
        var objectName = "test-object.txt";
        var content = "Hello, Minio!";
        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        await _bucketService.MakeBucketAsync(bucketName);

        // Act - Upload
            var uploadResult = await _objectService.UploadBytesAsync(bucketName, objectName, contentBytes, contentType: null, metadata: null, CancellationToken.None);

        // Assert - Upload
        uploadResult.Should().NotBeNull();
        uploadResult.Name.Should().Be(objectName);
        uploadResult.Size.Should().Be(contentBytes.Length);

        // Act - Download
        var downloadedBytes = await _objectService.DownloadAsBytesAsync(bucketName, objectName, progress: null, CancellationToken.None);

        // Assert - Download
        downloadedBytes.Should().NotBeNull();
        var downloadedContent = System.Text.Encoding.UTF8.GetString(downloadedBytes);
        downloadedContent.Should().Be(content);
    }

    [Fact]
    public async Task UploadAndDownloadFile_ShouldSucceed()
    {
        // Arrange
        var bucketName = $"test-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";
        var objectName = "test-file.txt";
        var tempFilePath = Path.GetTempFileName();
        var content = "Hello, Minio from file!";
        var downloadFilePath = Path.GetTempFileName();

        try
        {
            await File.WriteAllTextAsync(tempFilePath, content);
            await _bucketService.MakeBucketAsync(bucketName);

            // Act - Upload file
            var uploadResult = await _objectService.UploadFileAsync(bucketName, objectName, tempFilePath, CancellationToken.None);

            // Assert - Upload
            uploadResult.Should().NotBeNull();
            uploadResult.Name.Should().Be(objectName);

            // Act - Download file
            await _objectService.DownloadToFileAsync(bucketName, objectName, downloadFilePath, CancellationToken.None);

            // Assert - Download
            var downloadedContent = await File.ReadAllTextAsync(downloadFilePath);
            downloadedContent.Should().Be(content);
        }
        finally
        {
            if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
            if (File.Exists(downloadFilePath)) File.Delete(downloadFilePath);
        }
    }

    [Fact]
    public async Task UploadWithMetadata_ShouldSucceed()
    {
        // Arrange
        var bucketName = $"test-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";
        var objectName = "test-object-with-metadata.txt";
        var content = "Hello, Minio with metadata!";
        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);
        var metadata = new Dictionary<string, string>
        {
            { "Author", "Integration Test" },
            { "Content-Type", "text/plain" },
            { "Environment", "Test" }
        };

        await _bucketService.MakeBucketAsync(bucketName);

        // Act
        var uploadResult = await _objectService.UploadBytesAsync(bucketName, objectName, contentBytes, "text/plain", metadata);

        // Assert
        uploadResult.Should().NotBeNull();
        uploadResult.Name.Should().Be(objectName);

        // Verify metadata
        var objectMetadata = await _objectService.StatObjectAsync(bucketName, objectName);
        objectMetadata.Should().NotBeNull();
        objectMetadata.Name.Should().Be(objectName);
    }

    [Fact]
    public async Task CopyObject_ShouldSucceed()
    {
        // Arrange
        var bucketName = $"test-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";
        var sourceObjectName = "source-object.txt";
        var destObjectName = "dest-object.txt";
        var content = "Hello, Minio copy!";
        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        await _bucketService.MakeBucketAsync(bucketName);
        await _objectService.UploadBytesAsync(bucketName, sourceObjectName, contentBytes, contentType: null, metadata: null, CancellationToken.None);

        var copyRequest = new CopyObjectRequest
        {
            SourceBucketName = bucketName,
            SourceObjectName = sourceObjectName,
            DestinationBucketName = bucketName,
            DestinationObjectName = destObjectName
        };

        // Act
        await _objectService.CopyObjectAsync(copyRequest);

        // Assert - Verify the copied object exists and has correct content
        var copiedBytes = await _objectService.DownloadAsBytesAsync(bucketName, destObjectName, progress: null, CancellationToken.None);
        var copiedContent = System.Text.Encoding.UTF8.GetString(copiedBytes);
        copiedContent.Should().Be(content);
    }

    [Fact]
    public async Task DeleteObject_ShouldSucceed()
    {
        // Arrange
        var bucketName = $"test-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";
        var objectName = "test-object-to-delete.txt";
        var content = "Hello, Minio to delete!";
        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        await _bucketService.MakeBucketAsync(bucketName);
        await _objectService.UploadBytesAsync(bucketName, objectName, contentBytes, contentType: null, metadata: null, CancellationToken.None);

        // Verify object exists
        var exists = await _objectService.StatObjectAsync(bucketName, objectName);
        exists.Should().NotBeNull();

        // Act
        await _objectService.RemoveObjectAsync(bucketName, objectName);

        // Assert
        var action = async () => await _objectService.StatObjectAsync(bucketName, objectName);
        await action.Should().ThrowAsync<Minio.Exceptions.ObjectNotFoundException>();
    }

    [Fact]
    public async Task PresignedUrl_ShouldWork()
    {
        // Arrange
        var bucketName = $"test-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";
        var objectName = "test-presigned-object.txt";
        var content = "Hello, Minio presigned!";
        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        await _bucketService.MakeBucketAsync(bucketName);
        await _objectService.UploadBytesAsync(bucketName, objectName, contentBytes, contentType: null, metadata: null, CancellationToken.None);

        var presignedRequest = new PresignedUrlRequest
        {
            BucketName = bucketName,
            ObjectName = objectName,
            ExpiresInSeconds = (int)TimeSpan.FromHours(1).TotalSeconds
        };

        // Act
        var presignedUrl = await _presignedUrlService.PresignedGetObjectAsync(presignedRequest);

        // Assert
        presignedUrl.Should().NotBeNull();
        presignedUrl.Url.Should().NotBeNullOrEmpty();
        presignedUrl.BucketName.Should().Be(bucketName);
        presignedUrl.ObjectName.Should().Be(objectName);
    }

    [Fact]
    public async Task FluentBuilder_ShouldWork()
    {
        // Arrange
        var bucketName = $"test-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";
        var objectName = "test-fluent-object.txt";
        var content = "Hello, Minio fluent!";
        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        await _bucketService.MakeBucketAsync(bucketName);

        var uploadRequest = bucketName
            .UploadObject(objectName, new MemoryStream(contentBytes))
            .WithContentType("text/plain")
            .WithMetadata("Author", "Fluent Test")
            .WithMetadata("Environment", "Test")
            .Build();

        // Act
        var uploadResult = await _objectService.UploadAsync(uploadRequest);

        // Assert
        uploadResult.Should().NotBeNull();
        uploadResult.Name.Should().Be(objectName);

        // Verify download with fluent builder
        var downloadConfig = bucketName
            .DownloadObject(objectName)
            .WithRange(0, 10) // First 10 bytes
            .Build();

        using var destination = new MemoryStream();
        await _objectService.DownloadAsync(downloadConfig, destination);

        var downloadedContent = System.Text.Encoding.UTF8.GetString(destination.ToArray());
        downloadedContent.Should().Be("Hello, Min");
    }

    [Fact]
    public async Task MultipartUpload_ShouldWork()
    {
        // Arrange
        var bucketName = $"test-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";
        var objectName = "test-multipart-object.txt";
        
        // Create a large content to trigger multipart upload
        var content = new string('A', 10 * 1024 * 1024); // 10MB
        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        await _bucketService.MakeBucketAsync(bucketName);

        var multipartRequest = new MultipartUploadRequest
        {
            BucketName = bucketName,
            ObjectName = objectName,
            Stream = new MemoryStream(contentBytes),
            ContentType = "text/plain",
            PartSize = 5 * 1024 * 1024 // 5MB parts
        };

        // Act
        var uploadResult = await _objectService.UploadMultipartAsync(multipartRequest);

        // Assert
        uploadResult.Should().NotBeNull();
        uploadResult.Name.Should().Be(objectName);
        uploadResult.Size.Should().Be(contentBytes.Length);

        // Verify download
        var downloadedBytes = await _objectService.DownloadAsBytesAsync(bucketName, objectName, progress: null);
        var downloadedContent = System.Text.Encoding.UTF8.GetString(downloadedBytes);
        downloadedContent.Should().Be(content);
    }
}
