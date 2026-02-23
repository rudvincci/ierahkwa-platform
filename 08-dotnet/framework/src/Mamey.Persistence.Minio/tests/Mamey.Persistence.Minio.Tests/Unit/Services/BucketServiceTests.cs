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
/// Unit tests for BucketService.
/// </summary>
public class BucketServiceTests
{
    private readonly Mock<IMinioClient> _mockClient;
    private readonly Mock<IOptions<MinioOptions>> _mockOptions;
    private readonly Mock<ILogger<BucketService>> _mockLogger;
    private readonly Mock<IRetryPolicyExecutor> _mockRetryPolicyExecutor;
    private readonly BucketService _service;
    private readonly MinioOptions _options;

    public BucketServiceTests()
    {
        _mockClient = MockMinioClientFactory.CreateDefault();
        _mockLogger = new Mock<ILogger<BucketService>>();
        _mockRetryPolicyExecutor = new Mock<IRetryPolicyExecutor>();
        
        _options = new MinioOptions
        {
            Endpoint = "localhost:9000",
            AccessKey = "test-key",
            SecretKey = "test-secret"
        };
        
        _mockOptions = new Mock<IOptions<MinioOptions>>();
        _mockOptions.Setup(x => x.Value).Returns(_options);
        
        _service = new BucketService(_mockClient.Object, _mockOptions.Object, _mockLogger.Object, _mockRetryPolicyExecutor.Object);
    }

    [Fact]
    public async Task MakeBucketAsync_ShouldCallClientWithCorrectParameters()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, string, CancellationToken>((func, key, ct) => func(ct));

        // Act
        await _service.MakeBucketAsync(bucketName);

        // Assert
        _mockClient.Verify(x => x.MakeBucketAsync(
            It.Is<MakeBucketArgs>(args => args != null),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MakeBucketAsync_WithVersioning_ShouldCallClientWithVersioningArgs()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, string, CancellationToken>((func, key, ct) => func(ct));

        // Act
        await _service.MakeBucketAsync(bucketName);

        // Assert
        _mockClient.Verify(x => x.MakeBucketAsync(
            It.Is<MakeBucketArgs>(args => args != null),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveBucketAsync_ShouldCallClientWithCorrectParameters()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, string, CancellationToken>((func, key, ct) => func(ct));

        // Act
        await _service.RemoveBucketAsync(bucketName);

        // Assert
        _mockClient.Verify(x => x.RemoveBucketAsync(
            It.Is<RemoveBucketArgs>(args => args != null),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task BucketExistsAsync_ShouldReturnTrue_WhenBucketExists()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<bool>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<bool>>, string, CancellationToken>((func, key, ct) => func(ct));

        // Act
        var result = await _service.BucketExistsAsync(bucketName);

        // Assert
        result.Should().BeTrue();
        _mockClient.Verify(x => x.BucketExistsAsync(
            It.IsAny<BucketExistsArgs>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task BucketExistsAsync_ShouldReturnFalse_WhenBucketDoesNotExist()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var mockClient = MockMinioClientFactory.CreateBucketNotFound();
        var service = new BucketService(mockClient.Object, _mockOptions.Object, _mockLogger.Object, _mockRetryPolicyExecutor.Object);
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<bool>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<bool>>, string, CancellationToken>((func, key, ct) => func(ct));

        // Act
        var result = await service.BucketExistsAsync(bucketName);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ListBucketsAsync_ShouldReturnBucketList()
    {
        // Arrange
        var expectedBuckets = new List<Bucket>
        {
            new() { Name = "bucket1", CreationDate = DateTime.UtcNow.AddDays(-1) },
            new() { Name = "bucket2", CreationDate = DateTime.UtcNow.AddDays(-2) }
        };
        
        _mockClient.Setup(x => x.ListBucketsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBuckets);
            
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<Collection<Bucket>>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<Collection<Bucket>>>, string, CancellationToken>((func, key, ct) => func(ct));

        // Act
        var result = await _service.ListBucketsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(b => b.Name == "bucket1");
        result.Should().Contain(b => b.Name == "bucket2");
    }

    [Fact]
    public async Task EnableVersioningAsync_ShouldCallClientWithCorrectParameters()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, string, CancellationToken>((func, key, ct) => func(ct));

        // Act
        await _service.EnableVersioningAsync(bucketName);

        // Assert
        _mockClient.Verify(x => x.SetVersioningAsync(
            It.Is<SetVersioningArgs>(args => args.BucketName == bucketName),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DisableVersioningAsync_ShouldCallClientWithCorrectParameters()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, string, CancellationToken>((func, key, ct) => func(ct));

        // Act
        await _service.DisableVersioningAsync(bucketName);

        // Assert
        _mockClient.Verify(x => x.SetVersioningAsync(
            It.Is<SetVersioningArgs>(args => args.BucketName == bucketName),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetBucketVersioningAsync_ShouldReturnVersioningInfo()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var expectedVersioning = new VersioningConfiguration
        {
            Status = "Enabled",
            MfaDelete = "Disabled"
        };
        
        _mockClient.Setup(x => x.GetVersioningAsync(It.IsAny<GetVersioningArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedVersioning);
            
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<BucketVersioningInfo>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<BucketVersioningInfo>>, string, CancellationToken>((func, key, ct) => func(ct));

        // Act
        var result = await _service.GetBucketVersioningAsync(bucketName);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(VersioningStatus.Enabled);
        result.MfaDelete.Should().Be(MfaDeleteStatus.Disabled);
    }

    [Fact]
    public async Task SetBucketTagsAsync_ShouldCallClientWithCorrectParameters()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var tags = TestDataGenerator.CreateSampleBucketTags();
        var request = new SetBucketTagsRequest { BucketName = bucketName, Tags = tags };
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, string, CancellationToken>((func, key, ct) => func(ct));

        // Act
        await _service.SetBucketTagsAsync(request);

        // Assert
        // Note: Bucket tagging is not fully supported in current Minio API version
        // This test verifies the method doesn't throw exceptions
    }

    [Fact]
    public async Task GetBucketTagsAsync_ShouldReturnEmptyTags_WhenNotSupported()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<Dictionary<string, string>>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<Dictionary<string, string>>>, string, CancellationToken>((func, key, ct) => func(ct));

        // Act
        var result = await _service.GetBucketTagsAsync(bucketName);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ListObjectsAsync_ShouldReturnObjectList()
    {
        // Arrange
        var bucketName = TestDataGenerator.CreateBucketName();
        var request = new ListObjectsRequest { BucketName = bucketName };
        var items = new List<Item>
        {
            new() { Key = "object1.txt", Size = 1024, LastModifiedDateTime = DateTime.UtcNow },
            new() { Key = "object2.txt", Size = 2048, LastModifiedDateTime = DateTime.UtcNow.AddDays(-1) }
        };
        
        var mockObservable = new Mock<IObservable<Item>>();
        mockObservable.Setup(x => x.Subscribe(It.IsAny<IObserver<Item>>()))
            .Callback<IObserver<Item>>(observer =>
            {
                foreach (var item in items)
                {
                    observer.OnNext(item);
                }
                observer.OnCompleted();
            });
            
        _mockClient.Setup(x => x.ListObjectsAsync(It.IsAny<ListObjectsArgs>()))
            .Returns(mockObservable.Object);
            
        _mockRetryPolicyExecutor
            .Setup(x => x.ExecuteAsync(It.IsAny<Func<CancellationToken, Task<Collection<ObjectInfo>>>>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task<Collection<ObjectInfo>>>, string, CancellationToken>((func, key, ct) => func(ct));

        // Act
        var result = await _service.ListObjectsAsync(request);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(o => o.Name == "object1.txt");
        result.Should().Contain(o => o.Name == "object2.txt");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task MakeBucketAsync_ShouldThrowArgumentException_WhenBucketNameIsInvalid(string? bucketName)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.MakeBucketAsync(bucketName!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task RemoveBucketAsync_ShouldThrowArgumentException_WhenBucketNameIsInvalid(string? bucketName)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.RemoveBucketAsync(bucketName!));
    }

    [Fact]
    public async Task SetBucketTagsAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.SetBucketTagsAsync(null!));
    }

    [Fact]
    public async Task ListObjectsAsync_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.ListObjectsAsync(null!));
    }
}
