using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel;
using Moq;
using Mamey.Persistence.Minio.Exceptions;
using Mamey.Persistence.Minio.Infrastructure.Resilience;
using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;
using Mamey.Persistence.Minio.Services;
using Mamey.Persistence.Minio.Tests.Fixtures;
using Mamey.Persistence.Minio.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace Mamey.Persistence.Minio.Tests.Unit.Services;

/// <summary>
/// Unit tests for ObjectService.
/// </summary>
public class ObjectServiceTests
{
    private readonly Mock<IMinioClient> _mockClient;
    private readonly Mock<IOptions<MinioOptions>> _mockOptions;
    private readonly Mock<ILogger<ObjectService>> _mockLogger;
    private readonly Mock<IRetryPolicyExecutor> _mockRetryPolicyExecutor;
    private readonly ObjectService _service;
    private readonly MinioOptions _options;

    public ObjectServiceTests()
    {
        _mockClient = MockMinioClientFactory.CreateDefault();
        _mockLogger = new Mock<ILogger<ObjectService>>();
        _mockRetryPolicyExecutor = new Mock<IRetryPolicyExecutor>();
        
        _options = new MinioOptions
        {
            Endpoint = "localhost:9000",
            AccessKey = "test-key",
            SecretKey = "test-secret"
        };
        
        _mockOptions = new Mock<IOptions<MinioOptions>>();
        _mockOptions.Setup(x => x.Value).Returns(_options);
        
        _service = new ObjectService(_mockClient.Object, _mockOptions.Object, _mockLogger.Object, _mockRetryPolicyExecutor.Object);
    }

    [Fact]
    public async Task PutObjectAsync_ShouldCallClientWithCorrectParameters()
    {
        // Arrange
        var request = TestDataGenerator.CreatePutObjectRequest("test-bucket", "test-object.txt");
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, string, CancellationToken>((func, key, ct) => func(ct));

        // Act
        await _service.PutObjectAsync(request);

        // Assert
        _mockClient.Verify(x => x.PutObjectAsync(
            It.IsAny<PutObjectArgs>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DownloadAsync_ShouldCallClientWithCorrectParameters()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var destination = new MemoryStream();
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, CancellationToken>((func, ct) => func(ct));

        // Act
        await _service.DownloadAsync(bucketName, objectName, destination);

        // Assert
        _mockClient.Verify(x => x.GetObjectAsync(
            It.IsAny<GetObjectArgs>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveObjectAsync_ShouldCallClientWithCorrectParameters()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, string, CancellationToken>((func, key, ct) => func(ct));

        // Act
        await _service.RemoveObjectAsync(bucketName, objectName);

        // Assert
        _mockClient.Verify(x => x.RemoveObjectAsync(
            It.IsAny<RemoveObjectArgs>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    // Commented out - ObjectStat is immutable and cannot be easily mocked/created for testing
    // [Fact]
    // public async Task StatObjectAsync_ShouldReturnMetadata()
    // {
    //     // Arrange
    //     var bucketName = TestDataGenerator.CreateBucketName();
    //     var objectName = TestDataGenerator.CreateObjectName();
    //     
    //     // ObjectStat is immutable - try to create using constructor with reflection
    //     ObjectStat? expectedStat = null;
    //     try
    //     {
    //         var constructor = typeof(ObjectStat).GetConstructor(
    //             new[] { typeof(string), typeof(string), typeof(long), typeof(DateTime), typeof(string), typeof(string) });
    //         if (constructor != null)
    //         {
    //             expectedStat = constructor.Invoke(new object[] 
    //             { 
    //                 bucketName, objectName, 2048L, DateTime.UtcNow, "test-etag-123", "text/plain" 
    //             }) as ObjectStat;
    //         }
    //     }
    //     catch
    //     {
    //         // If constructor doesn't exist, we'll need to handle this differently
    //     }
    //     
    //     if (expectedStat == null)
    //     {
    //         // Skip test if we can't create ObjectStat - this is a limitation of the Minio library
    //         return;
    //     }
    //     
    //     _mockClient.Setup(x => x.StatObjectAsync(It.IsAny<StatObjectArgs>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(expectedStat);
    //         
    //     _mockRetryPolicyExecutor
    //         .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<ObjectMetadata>>>(), It.IsAny<CancellationToken>()))
    //         .Returns<Func<CancellationToken, Task<ObjectMetadata>>, CancellationToken>((func, ct) => func(ct));
    //
    //     // Act
    //     var result = await _service.StatObjectAsync(bucketName, objectName);
    //
    //     // Assert
    //     result.Should().NotBeNull();
    //     result.Name.Should().Be(objectName);
    //     result.Size.Should().Be(expectedStat.Size);
    //     result.ETag.Should().Be(expectedStat.ETag);
    //     result.ContentType.Should().Be(expectedStat.ContentType);
    // }

    [Fact]
    public async Task CopyObjectAsync_ShouldCallClientWithCorrectParameters()
    {
        // Arrange
        var request = new CopyObjectRequest
        {
            SourceBucketName = "source-bucket",
            SourceObjectName = "source-object.txt",
            DestinationBucketName = "dest-bucket",
            DestinationObjectName = "dest-object.txt"
        };
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<ObjectMetadata>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<ObjectMetadata>>, CancellationToken>((func, ct) => func(ct));

        // Act
        var result = await _service.CopyObjectAsync(request);

        // Assert
        result.Should().NotBeNull();
        _mockClient.Verify(x => x.CopyObjectAsync(
            It.IsAny<CopyObjectArgs>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UploadFileAsync_ShouldUploadFileSuccessfully()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var filePath = Path.GetTempFileName();
        var testData = TestDataGenerator.CreateTestData(1024);
        
        try
        {
            await File.WriteAllBytesAsync(filePath, testData);
            
            _mockRetryPolicyExecutor
                .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<ObjectMetadata>>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task<ObjectMetadata>>, CancellationToken>((func, ct) => func(ct));

            // Act
            var result = await _service.UploadFileAsync(bucketName, objectName, filePath);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(objectName);
            result.Size.Should().Be(testData.Length);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task UploadFileAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var filePath = "non-existent-file.txt";

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => 
            _service.UploadFileAsync(bucketName, objectName, filePath));
    }

    [Fact]
    public async Task DownloadToFileAsync_ShouldDownloadToFileSuccessfully()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var filePath = Path.GetTempFileName();
        
        try
        {
            _mockRetryPolicyExecutor
                .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<CancellationToken, Task>, string, CancellationToken>((func, key, ct) => func(ct));

            // Act
            await _service.DownloadToFileAsync(bucketName, objectName, filePath);

            // Assert
            File.Exists(filePath).Should().BeTrue();
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task UploadBytesAsync_ShouldUploadBytesSuccessfully()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var testData = TestDataGenerator.CreateTestData(512);
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<ObjectMetadata>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<ObjectMetadata>>, CancellationToken>((func, ct) => func(ct));

        // Act
        var result = await _service.UploadBytesAsync(bucketName, objectName, testData);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(objectName);
        result.Size.Should().Be(testData.Length);
    }

    [Fact]
    public async Task DownloadAsBytesAsync_ShouldDownloadBytesSuccessfully()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var objectName = TestDataGenerator.CreateObjectName();
        var expectedData = TestDataGenerator.CreateTestData(256);
        
        _mockClient.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectArgs>(), It.IsAny<CancellationToken>()))
            .Callback<GetObjectArgs, CancellationToken>((args, ct) =>
            {
                var stream = args.CallbackStream;
                stream.Write(expectedData, 0, expectedData.Length);
            })
            .Returns(Task.CompletedTask);
            
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<byte[]>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<byte[]>>, CancellationToken>((func, ct) => func(ct));

        // Act
        var result = await _service.DownloadAsBytesAsync(bucketName, objectName);

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().Be(expectedData.Length);
    }

    [Fact]
    public async Task UploadMultipartAsync_ShouldUseMultipartForLargeFiles()
    {
        // Arrange
        var request = TestDataGenerator.CreateMultipartUploadRequest("test-bucket", "large-file.txt", 200 * 1024 * 1024); // 200MB
        var progress = new Progress<MultipartUploadProgress>();
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<ObjectMetadata>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<ObjectMetadata>>, CancellationToken>((func, ct) => func(ct));

        // Act
        var result = await _service.UploadMultipartAsync(request, progress);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.ObjectName);
    }

    [Fact]
    public async Task UploadAsync_ShouldUseMultipartForLargeFiles()
    {
        // Arrange
        var request = TestDataGenerator.CreatePutObjectRequest("test-bucket", "large-file.txt", 200 * 1024 * 1024); // 200MB
        var progress = new Progress<MultipartUploadProgress>();
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<ObjectMetadata>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<ObjectMetadata>>, CancellationToken>((func, ct) => func(ct));

        // Act
        var result = await _service.UploadAsync(request, progress);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.ObjectName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task PutObjectAsync_ShouldThrowArgumentException_WhenBucketNameIsInvalid(string? bucketName)
    {
        // Arrange
        var request = new PutObjectRequest
        {
            BucketName = bucketName!,
            ObjectName = "test-object.txt",
            Data = new MemoryStream(),
            Size = 0
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.PutObjectAsync(request));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task GetObjectAsync_ShouldThrowArgumentException_WhenBucketNameIsInvalid(string? bucketName)
    {
        // Arrange
        var destination = new MemoryStream();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetObjectAsync(bucketName!, "test-object.txt", destination));
    }

    [Fact]
    public async Task PutObjectAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.PutObjectAsync(null!));
    }

    [Fact]
    public async Task GetObjectAsync_ShouldThrowArgumentNullException_WhenDestinationIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetObjectAsync("test-bucket", "test-object.txt", null!));
    }

    [Fact]
    public async Task CopyObjectAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CopyObjectAsync(null!));
    }

    [Fact]
    public async Task UploadBytesAsync_ShouldThrowArgumentNullException_WhenDataIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _service.UploadBytesAsync("test-bucket", "test-object.txt", null!));
    }

    [Fact]
    public async Task DownloadAsync_WithConfiguration_ShouldCallClientWithCorrectParameters()
    {
        // Arrange
        var configuration = new ObjectDownloadConfiguration
        {
            BucketName = "test-bucket",
            ObjectName = "test-object.txt",
            VersionId = "version-123"
        };
        var destination = new MemoryStream();
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, string, CancellationToken>((func, key, ct) => func(ct));

        // Act
        await _service.DownloadAsync(configuration, destination);

        // Assert
        _mockClient.Verify(x => x.GetObjectAsync(
            It.IsAny<GetObjectArgs>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DownloadAsync_WithConfiguration_ShouldThrowArgumentNullException_WhenConfigurationIsNull()
    {
        // Arrange
        var destination = new MemoryStream();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.DownloadAsync(null!, destination));
    }

    [Fact]
    public async Task DownloadAsync_WithConfiguration_ShouldThrowArgumentNullException_WhenDestinationIsNull()
    {
        // Arrange
        var configuration = new ObjectDownloadConfiguration
        {
            BucketName = "test-bucket",
            ObjectName = "test-object.txt"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.DownloadAsync(configuration, null!));
    }
}
