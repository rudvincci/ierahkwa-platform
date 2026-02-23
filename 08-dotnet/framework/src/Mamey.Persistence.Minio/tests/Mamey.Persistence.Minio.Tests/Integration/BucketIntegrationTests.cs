using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Mamey.Persistence.Minio;
using Mamey.Persistence.Minio.Infrastructure.Resilience;
using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;
using Testcontainers.Minio;
using Xunit;

namespace Mamey.Persistence.Minio.Tests.Integration;

/// <summary>
/// Integration tests for bucket operations using TestContainers.
/// </summary>
public class BucketIntegrationTests : IClassFixture<MinioContainerFixture>
{
    private readonly MinioContainerFixture _fixture;
    private readonly IBucketService _bucketService;
    private readonly IObjectService _objectService;

    public BucketIntegrationTests(MinioContainerFixture fixture)
    {
        _fixture = fixture;
        _bucketService = _fixture.ServiceProvider.GetRequiredService<IBucketService>();
        _objectService = _fixture.ServiceProvider.GetRequiredService<IObjectService>();
    }

    [Fact]
    public async Task CreateBucket_ShouldSucceed()
    {
        // Arrange
        var bucketName = $"test-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";

        // Act
        await _bucketService.MakeBucketAsync(bucketName);

        // Assert
        var exists = await _bucketService.BucketExistsAsync(bucketName);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ListBuckets_ShouldReturnCreatedBuckets()
    {
        // Arrange
        var bucketName1 = $"test-bucket-1-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";
        var bucketName2 = $"test-bucket-2-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";

        await _bucketService.MakeBucketAsync(bucketName1);
        await _bucketService.MakeBucketAsync(bucketName2);

        // Act
        var buckets = await _bucketService.ListBucketsAsync();

        // Assert
        buckets.Should().NotBeNull();
        buckets.Should().Contain(b => b.Name == bucketName1);
        buckets.Should().Contain(b => b.Name == bucketName2);
    }

    [Fact]
    public async Task DeleteBucket_ShouldSucceed()
    {
        // Arrange
        var bucketName = $"test-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";
        await _bucketService.MakeBucketAsync(bucketName);

        // Act
        await _bucketService.RemoveBucketAsync(bucketName);

        // Assert
        var exists = await _bucketService.BucketExistsAsync(bucketName);
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task BucketExists_ShouldReturnCorrectStatus()
    {
        // Arrange
        var existingBucket = $"test-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";
        var nonExistingBucket = $"non-existing-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";

        await _bucketService.MakeBucketAsync(existingBucket);

        // Act & Assert
        var exists = await _bucketService.BucketExistsAsync(existingBucket);
        exists.Should().BeTrue();

        exists = await _bucketService.BucketExistsAsync(nonExistingBucket);
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task SetBucketVersioning_ShouldSucceed()
    {
        // Arrange
        var bucketName = $"test-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";
        await _bucketService.MakeBucketAsync(bucketName);

        // Act
        await _bucketService.SetBucketVersioningAsync(bucketName, VersioningStatus.Enabled);

        // Assert
        var versioning = await _bucketService.GetBucketVersioningAsync(bucketName);
        versioning.Should().NotBeNull();
        versioning.Status.Should().Be(VersioningStatus.Enabled);
    }

    [Fact]
    public async Task SetBucketTags_ShouldSucceed()
    {
        // Arrange
        var bucketName = $"test-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";
        await _bucketService.MakeBucketAsync(bucketName);

        var tags = new Dictionary<string, string>
        {
            { "Environment", "Test" },
            { "Project", "Mamey" },
            { "Owner", "IntegrationTest" }
        };

        // Act
        await _bucketService.SetBucketTagsAsync(bucketName, tags);

        // Assert
        var retrievedTags = await _bucketService.GetBucketTagsAsync(bucketName);
        retrievedTags.Should().NotBeNull();
        retrievedTags.TagSet.Should().Contain(t => t.Key == "Environment" && t.Value == "Test");
        retrievedTags.TagSet.Should().Contain(t => t.Key == "Project" && t.Value == "Mamey");
        retrievedTags.TagSet.Should().Contain(t => t.Key == "Owner" && t.Value == "IntegrationTest");
    }

    [Fact]
    public async Task ListObjects_ShouldReturnObjectsInBucket()
    {
        // Arrange
        var bucketName = $"test-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";
        await _bucketService.MakeBucketAsync(bucketName);

        var objectName1 = "test-object-1.txt";
        var objectName2 = "test-object-2.txt";
        var content1 = "Hello World 1";
        var content2 = "Hello World 2";

        await _objectService.UploadBytesAsync(bucketName, objectName1, System.Text.Encoding.UTF8.GetBytes(content1), contentType: null, metadata: null, CancellationToken.None);
        await _objectService.UploadBytesAsync(bucketName, objectName2, System.Text.Encoding.UTF8.GetBytes(content2), contentType: null, metadata: null, CancellationToken.None);

        // Act
        var objects = await _bucketService.ListObjectsAsync(new ListObjectsRequest { BucketName = bucketName });

        // Assert
        objects.Should().NotBeNull();
        objects.Should().HaveCount(2);
        objects.Should().Contain(o => o.Name == objectName1);
        objects.Should().Contain(o => o.Name == objectName2);
    }

    [Fact]
    public async Task ListObjects_WithPrefix_ShouldReturnFilteredObjects()
    {
        // Arrange
        var bucketName = $"test-bucket-{Guid.NewGuid().ToString().ToLowerInvariant().Replace("-", "")}";
        await _bucketService.MakeBucketAsync(bucketName);

        var objectName1 = "documents/file1.txt";
        var objectName2 = "images/photo1.jpg";
        var objectName3 = "documents/file2.txt";

        await _objectService.UploadBytesAsync(bucketName, objectName1, System.Text.Encoding.UTF8.GetBytes("content1"), contentType: null, metadata: null, CancellationToken.None);
        await _objectService.UploadBytesAsync(bucketName, objectName2, System.Text.Encoding.UTF8.GetBytes("content2"), contentType: null, metadata: null, CancellationToken.None);
        await _objectService.UploadBytesAsync(bucketName, objectName3, System.Text.Encoding.UTF8.GetBytes("content3"), contentType: null, metadata: null, CancellationToken.None);

        // Act
        var objects = await _bucketService.ListObjectsAsync(new ListObjectsRequest { BucketName = bucketName, Prefix = "documents/" });

        // Assert
        objects.Should().NotBeNull();
        objects.Should().HaveCount(2);
        objects.Should().Contain(o => o.Name == objectName1);
        objects.Should().Contain(o => o.Name == objectName3);
        objects.Should().NotContain(o => o.Name == objectName2);
    }
}
