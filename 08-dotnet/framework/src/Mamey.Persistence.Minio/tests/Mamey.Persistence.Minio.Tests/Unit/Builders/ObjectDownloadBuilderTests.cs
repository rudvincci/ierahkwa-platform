using Mamey.Persistence.Minio.Builders;
using Mamey.Persistence.Minio.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace Mamey.Persistence.Minio.Tests.Unit.Builders;

/// <summary>
/// Unit tests for ObjectDownloadBuilder.
/// </summary>
public class ObjectDownloadBuilderTests
{
    [Fact]
    public void Build_ShouldCreateDownloadConfiguration_WithBasicProperties()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .Build();

        // Assert
        config.Should().NotBeNull();
        config.BucketName.Should().Be(bucketName);
        config.ObjectName.Should().Be(objectName);
        config.VersionId.Should().BeNull();
        config.Headers.Should().BeNull();
    }

    [Fact]
    public void WithVersion_ShouldSetVersionId()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var versionId = "version-123";

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .WithVersion(versionId)
            .Build();

        // Assert
        config.VersionId.Should().Be(versionId);
    }

    [Fact]
    public void WithRange_ShouldAddRangeHeader()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var start = 1024L;
        var end = 2048L;

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .WithRange(start, end)
            .Build();

        // Assert
        config.Headers.Should().NotBeNull();
        config.Headers.Should().ContainKey("Range");
        config.Headers["Range"].Should().Be("bytes=1024-2048");
    }

    [Fact]
    public void WithRange_ShouldAddRangeHeader_WhenEndIsNull()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var start = 1024L;

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .WithRange(start)
            .Build();

        // Assert
        config.Headers.Should().NotBeNull();
        config.Headers.Should().ContainKey("Range");
        config.Headers["Range"].Should().Be("bytes=1024-");
    }

    [Fact]
    public void FromByte_ShouldAddRangeHeader_FromStartToEnd()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var start = 1024L;

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .FromByte(start)
            .Build();

        // Assert
        config.Headers.Should().NotBeNull();
        config.Headers.Should().ContainKey("Range");
        config.Headers["Range"].Should().Be("bytes=1024-");
    }

    [Fact]
    public void WithLength_ShouldAddRangeHeader_WithLength()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var start = 1024L;
        var length = 512L;

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .WithLength(start, length)
            .Build();

        // Assert
        config.Headers.Should().NotBeNull();
        config.Headers.Should().ContainKey("Range");
        config.Headers["Range"].Should().Be("bytes=1024-1535");
    }

    [Fact]
    public void IfMatch_ShouldAddIfMatchHeader()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var etag = "etag-123";

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .IfMatch(etag)
            .Build();

        // Assert
        config.Headers.Should().NotBeNull();
        config.Headers.Should().ContainKey("If-Match");
        config.Headers["If-Match"].Should().Be(etag);
    }

    [Fact]
    public void IfNoneMatch_ShouldAddIfNoneMatchHeader()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var etag = "etag-123";

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .IfNoneMatch(etag)
            .Build();

        // Assert
        config.Headers.Should().NotBeNull();
        config.Headers.Should().ContainKey("If-None-Match");
        config.Headers["If-None-Match"].Should().Be(etag);
    }

    [Fact]
    public void IfModifiedSince_ShouldAddIfModifiedSinceHeader()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var date = DateTime.UtcNow.AddDays(-1);

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .IfModifiedSince(date)
            .Build();

        // Assert
        config.Headers.Should().NotBeNull();
        config.Headers.Should().ContainKey("If-Modified-Since");
        config.Headers["If-Modified-Since"].Should().Be(date.ToString("R"));
    }

    [Fact]
    public void IfUnmodifiedSince_ShouldAddIfUnmodifiedSinceHeader()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var date = DateTime.UtcNow.AddDays(-1);

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .IfUnmodifiedSince(date)
            .Build();

        // Assert
        config.Headers.Should().NotBeNull();
        config.Headers.Should().ContainKey("If-Unmodified-Since");
        config.Headers["If-Unmodified-Since"].Should().Be(date.ToString("R"));
    }

    [Fact]
    public void WithHeader_ShouldAddCustomHeader()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .WithHeader("Custom-Header", "custom-value")
            .Build();

        // Assert
        config.Headers.Should().NotBeNull();
        config.Headers.Should().ContainKey("Custom-Header");
        config.Headers["Custom-Header"].Should().Be("custom-value");
    }

    [Fact]
    public void WithHeaders_ShouldAddMultipleHeaders()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var headers = new Dictionary<string, string>
        {
            { "Header1", "Value1" },
            { "Header2", "Value2" }
        };

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .WithHeaders(headers)
            .Build();

        // Assert
        config.Headers.Should().NotBeNull();
        config.Headers.Should().ContainKey("Header1");
        config.Headers.Should().ContainKey("Header2");
        config.Headers["Header1"].Should().Be("Value1");
        config.Headers["Header2"].Should().Be("Value2");
    }

    [Fact]
    public void WithResponseContentType_ShouldAddResponseContentTypeHeader()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var contentType = "application/pdf";

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .WithResponseContentType(contentType)
            .Build();

        // Assert
        config.Headers.Should().NotBeNull();
        config.Headers.Should().ContainKey("response-content-type");
        config.Headers["response-content-type"].Should().Be(contentType);
    }

    [Fact]
    public void WithResponseContentDisposition_ShouldAddResponseContentDispositionHeader()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var contentDisposition = "attachment; filename=\"document.pdf\"";

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .WithResponseContentDisposition(contentDisposition)
            .Build();

        // Assert
        config.Headers.Should().NotBeNull();
        config.Headers.Should().ContainKey("response-content-disposition");
        config.Headers["response-content-disposition"].Should().Be(contentDisposition);
    }

    [Fact]
    public void WithResponseCacheControl_ShouldAddResponseCacheControlHeader()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var cacheControl = "max-age=3600";

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .WithResponseCacheControl(cacheControl)
            .Build();

        // Assert
        config.Headers.Should().NotBeNull();
        config.Headers.Should().ContainKey("response-cache-control");
        config.Headers["response-cache-control"].Should().Be(cacheControl);
    }

    [Fact]
    public void Build_ShouldCombineAllHeaders()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var versionId = "version-123";
        var etag = "etag-123";
        var date = DateTime.UtcNow.AddDays(-1);

        // Act
        var config = bucketName
            .DownloadObject(objectName)
            .WithVersion(versionId)
            .WithRange(1024, 2048)
            .IfMatch(etag)
            .IfModifiedSince(date)
            .WithResponseContentType("application/pdf")
            .WithResponseContentDisposition("attachment")
            .Build();

        // Assert
        config.VersionId.Should().Be(versionId);
        config.Headers.Should().NotBeNull();
        config.Headers.Should().ContainKey("Range");
        config.Headers.Should().ContainKey("If-Match");
        config.Headers.Should().ContainKey("If-Modified-Since");
        config.Headers.Should().ContainKey("response-content-type");
        config.Headers.Should().ContainKey("response-content-disposition");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void DownloadObject_ShouldThrowArgumentException_WhenBucketNameIsInvalid(string? bucketName)
    {
        // Arrange
        var objectName = TestDataGenerator.CreateObjectName();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName!.DownloadObject(objectName));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void DownloadObject_ShouldThrowArgumentException_WhenObjectNameIsInvalid(string? objectName)
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName.DownloadObject(objectName!));
    }

    [Fact]
    public void WithHeader_ShouldThrowArgumentException_WhenKeyIsNull()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .DownloadObject(objectName)
            .WithHeader(null!, "value"));
    }

    [Fact]
    public void WithHeader_ShouldThrowArgumentException_WhenValueIsNull()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .DownloadObject(objectName)
            .WithHeader("key", null!));
    }

    [Fact]
    public void WithHeaders_ShouldThrowArgumentNullException_WhenHeadersIsNull()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => bucketName
            .DownloadObject(objectName)
            .WithHeaders(null!));
    }

    [Fact]
    public void WithLength_ShouldThrowArgumentException_WhenLengthIsNegative()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .DownloadObject(objectName)
            .WithLength(1024, -1));
    }

    [Fact]
    public void WithLength_ShouldThrowArgumentException_WhenStartIsNegative()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => bucketName
            .DownloadObject(objectName)
            .WithLength(-1, 1024));
    }
}
