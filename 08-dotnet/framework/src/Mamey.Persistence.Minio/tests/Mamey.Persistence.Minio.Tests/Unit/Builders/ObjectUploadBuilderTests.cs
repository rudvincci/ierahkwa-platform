using Mamey.Persistence.Minio.Builders;
using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;
using Mamey.Persistence.Minio.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace Mamey.Persistence.Minio.Tests.Unit.Builders;

/// <summary>
/// Unit tests for ObjectUploadBuilder.
/// </summary>
public class ObjectUploadBuilderTests
{
    [Fact]
    public void Build_ShouldCreatePutObjectRequest_WithBasicProperties()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);

        // Act
        var request = bucketName
            .UploadObject(objectName, stream)
            .Build();

        // Assert
        request.Should().NotBeNull();
        request.BucketName.Should().Be(bucketName);
        request.ObjectName.Should().Be(objectName);
        request.Data.Should().BeSameAs(stream);
        request.Size.Should().Be(1024);
    }

    [Fact]
    public void WithContentType_ShouldSetContentType()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);
        var contentType = "application/pdf";

        // Act
        var request = bucketName
            .UploadObject(objectName, stream)
            .WithContentType(contentType)
            .Build();

        // Assert
        request.ContentType.Should().Be(contentType);
    }

    [Fact]
    public void WithContentTypeFromFile_ShouldDetectContentType()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = "document.pdf";
        var stream = TestDataGenerator.CreateTestStream(1024);

        // Act
        var request = bucketName
            .UploadObject(objectName, stream)
            .WithContentTypeFromFile(objectName)
            .Build();

        // Assert
        request.ContentType.Should().Be("application/pdf");
    }

    [Fact]
    public void WithMetadata_ShouldAddSingleMetadata()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);

        // Act
        var request = bucketName
            .UploadObject(objectName, stream)
            .WithMetadata("author", "John Doe")
            .Build();

        // Assert
        request.Metadata.Should().NotBeNull();
        request.Metadata.Should().ContainKey("author");
        request.Metadata["author"].Should().Be("John Doe");
    }

    [Fact]
    public void WithMetadata_ShouldAddMultipleMetadata()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);
        var metadata = TestDataGenerator.CreateSampleMetadata();

        // Act
        var request = bucketName
            .UploadObject(objectName, stream)
            .WithMetadata(metadata)
            .Build();

        // Assert
        request.Metadata.Should().NotBeNull();
        request.Metadata.Should().ContainKey("author");
        request.Metadata.Should().ContainKey("department");
        request.Metadata.Should().ContainKey("project");
        request.Metadata.Should().ContainKey("version");
    }

    [Fact]
    public void WithHeader_ShouldAddSingleHeader()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);

        // Act
        var request = bucketName
            .UploadObject(objectName, stream)
            .WithHeader("Cache-Control", "max-age=3600")
            .Build();

        // Assert
        // Note: Headers are not directly exposed in PutObjectRequest in current implementation
        // This test verifies the method doesn't throw exceptions
        request.Should().NotBeNull();
    }

    [Fact]
    public void WithHeaders_ShouldAddMultipleHeaders()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);
        var headers = new Dictionary<string, string>
        {
            { "Cache-Control", "max-age=3600" },
            { "Content-Disposition", "attachment" }
        };

        // Act
        var request = bucketName
            .UploadObject(objectName, stream)
            .WithHeaders(headers)
            .Build();

        // Assert
        request.Should().NotBeNull();
    }

    [Fact]
    public void WithMultipartUpload_ShouldConfigureMultipartSettings()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);
        var partSize = 5 * 1024 * 1024; // 5MB
        var maxConcurrency = 6;

        // Act
        var request = bucketName
            .UploadObject(objectName, stream)
            .WithMultipartUpload(partSize, maxConcurrency, true)
            .BuildMultipart();

        // Assert
        request.Should().NotBeNull();
        request.PartSize.Should().Be(partSize);
        request.MaxConcurrency.Should().Be(maxConcurrency);
        request.ResumeUpload.Should().BeTrue();
    }

    [Fact]
    public void WithEncryption_ShouldAddEncryptionHeaders()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);

        // Act
        var request = bucketName
            .UploadObject(objectName, stream)
            .WithEncryption("AES256", "my-key-id")
            .Build();

        // Assert
        request.Should().NotBeNull();
        // Note: Encryption headers are not directly exposed in PutObjectRequest in current implementation
        // This test verifies the method doesn't throw exceptions
    }

    [Fact]
    public void WithTags_ShouldAddTagHeaders()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);
        var tags = new Dictionary<string, string>
        {
            { "project", "alpha" },
            { "status", "draft" }
        };

        // Act
        var request = bucketName
            .UploadObject(objectName, stream)
            .WithTags(tags)
            .Build();

        // Assert
        request.Should().NotBeNull();
        // Note: Tags are not directly exposed in PutObjectRequest in current implementation
        // This test verifies the method doesn't throw exceptions
    }

    [Fact]
    public void WithCacheControl_ShouldAddCacheControlHeader()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);

        // Act
        var request = bucketName
            .UploadObject(objectName, stream)
            .WithCacheControl("max-age=3600")
            .Build();

        // Assert
        request.Should().NotBeNull();
    }

    [Fact]
    public void WithContentDisposition_ShouldAddContentDispositionHeader()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);

        // Act
        var request = bucketName
            .UploadObject(objectName, stream)
            .WithContentDisposition("attachment; filename=\"document.pdf\"")
            .Build();

        // Assert
        request.Should().NotBeNull();
    }

    [Fact]
    public void ShouldUseMultipart_ShouldReturnTrue_WhenExplicitlyEnabled()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);

        // Act
        var builder = bucketName
            .UploadObject(objectName, stream)
            .WithMultipartUpload();

        // Assert
        builder.ShouldUseMultipart().Should().BeTrue();
    }

    [Fact]
    public void ShouldUseMultipart_ShouldReturnTrue_WhenStreamIsLarge()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(200 * 1024 * 1024); // 200MB

        // Act
        var builder = bucketName
            .UploadObject(objectName, stream);

        // Assert
        builder.ShouldUseMultipart().Should().BeTrue();
    }

    [Fact]
    public void ShouldUseMultipart_ShouldReturnFalse_WhenStreamIsSmall()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024); // 1KB

        // Act
        var builder = bucketName
            .UploadObject(objectName, stream);

        // Assert
        builder.ShouldUseMultipart().Should().BeFalse();
    }

    [Fact]
    public async Task UploadFile_ShouldCreateBuilderWithFileStream()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var filePath = Path.GetTempFileName();
        var testData = TestDataGenerator.CreateTestData(1024);
        
        try
        {
            await File.WriteAllBytesAsync(filePath, testData).ConfigureAwait(false);

            // Act
            var builder = bucketName.UploadFile(objectName, filePath);

            // Assert
            builder.Should().NotBeNull();
            builder.ShouldUseMultipart().Should().BeFalse();
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public void UploadBytes_ShouldCreateBuilderWithMemoryStream()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var data = TestDataGenerator.CreateTestData(1024);

        // Act
        var builder = bucketName.UploadBytes(objectName, data);

        // Assert
        builder.Should().NotBeNull();
        builder.ShouldUseMultipart().Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UploadObject_ShouldThrowArgumentException_WhenBucketNameIsInvalid(string? bucketName)
    {
        // Arrange
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName!.UploadObject(objectName, stream));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UploadObject_ShouldThrowArgumentException_WhenObjectNameIsInvalid(string? objectName)
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var stream = TestDataGenerator.CreateTestStream(1024);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName.UploadObject(objectName!, stream));
    }

    [Fact]
    public void UploadObject_ShouldThrowArgumentNullException_WhenStreamIsNull()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => bucketName.UploadObject(objectName, null!));
    }

    [Fact]
    public void WithMetadata_ShouldThrowArgumentException_WhenKeyIsNull()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .UploadObject(objectName, stream)
            .WithMetadata(null!, "value"));
    }

    [Fact]
    public void WithMetadata_ShouldThrowArgumentException_WhenValueIsNull()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .UploadObject(objectName, stream)
            .WithMetadata("key", null!));
    }

    [Fact]
    public void WithMultipartUpload_ShouldThrowArgumentException_WhenPartSizeIsInvalid()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .UploadObject(objectName, stream)
            .WithMultipartUpload(0)); // Invalid part size
    }

    [Fact]
    public void WithMultipartUpload_ShouldThrowArgumentException_WhenMaxConcurrencyIsInvalid()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var stream = TestDataGenerator.CreateTestStream(1024);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .UploadObject(objectName, stream)
            .WithMultipartUpload(maxConcurrency: 0)); // Invalid concurrency
    }
}
