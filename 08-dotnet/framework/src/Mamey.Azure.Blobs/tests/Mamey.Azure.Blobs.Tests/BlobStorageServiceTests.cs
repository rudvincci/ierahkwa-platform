using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Moq;
using System.Reflection;

namespace Mamey.Azure.Blobs.Tests;

public class BlobStorageServiceAccountOperationsTests
{
    private readonly Mock<BlobServiceClient> _blobServiceClientMock;
    private readonly BlobStorageService _blobStorageService;

    public BlobStorageServiceAccountOperationsTests()
    {
        _blobServiceClientMock = new Mock<BlobServiceClient>();
        _blobStorageService = new BlobStorageService(_blobServiceClientMock.Object);
    }

    [Fact]
    public async Task SetBlobServicePropertiesAsync_ShouldCallSetPropertiesAsync()
    {
        // Arrange
        var properties = new BlobServiceProperties();
        var responseMock = Mock.Of<Response>();
        _blobServiceClientMock.Setup(x => x.SetPropertiesAsync(properties, default))
                              .ReturnsAsync(responseMock);

        // Act
        await _blobStorageService.SetBlobServicePropertiesAsync(properties);

        // Assert
        _blobServiceClientMock.Verify(x => x.SetPropertiesAsync(properties, default), Times.Once);
    }

    [Fact]
    public async Task GetBlobServicePropertiesAsync_ShouldReturnBlobServiceProperties()
    {
        // Arrange
        var expectedProperties = new BlobServiceProperties();
        var responseMock = Mock.Of<Response<BlobServiceProperties>>(r => r.Value == expectedProperties);
        _blobServiceClientMock.Setup(x => x.GetPropertiesAsync(default))
                              .ReturnsAsync(responseMock);

        // Act
        var properties = await _blobStorageService.GetBlobServicePropertiesAsync();

        // Assert
        Assert.Equal(expectedProperties, properties);
    }

    [Fact]
    public async Task PreflightBlobRequestAsync_ShouldReturnCorsRules()
    {
        // Arrange
        var corsRules = new BlobCorsRule[] { new BlobCorsRule { AllowedOrigins = "*", AllowedMethods = "GET" } };
        var expectedProperties = new BlobServiceProperties { Cors = corsRules };
        var responseMock = Mock.Of<Response<BlobServiceProperties>>(r => r.Value == expectedProperties);
        _blobServiceClientMock.Setup(x => x.GetPropertiesAsync(default))
                              .ReturnsAsync(responseMock);

        // Act
        var result = await _blobStorageService.PreflightBlobRequestAsync();

        // Assert
        Assert.Equal(corsRules, result);
    }

    [Fact]
    public async Task GetBlobServiceStatsAsync_ShouldReturnBlobServiceStatistics()
    {
        // Arrange
        var geoReplication = CreateBlobGeoReplication(BlobGeoReplicationStatus.Live, DateTimeOffset.UtcNow);
        var expectedStats = CreateBlobServiceStatistics(geoReplication);
        var responseMock = Mock.Of<Response<BlobServiceStatistics>>(r => r.Value == expectedStats);
        _blobServiceClientMock.Setup(x => x.GetStatisticsAsync(default))
                              .ReturnsAsync(responseMock);

        // Act
        var stats = await _blobStorageService.GetBlobServiceStatsAsync();

        // Assert
        Assert.Equal(expectedStats.GeoReplication.Status, stats.GeoReplication.Status);
    }

    [Fact]
    public async Task GetAccountInfoAsync_ShouldReturnAccountInfo()
    {
        // Arrange
        var expectedAccountInfo = Mock.Of<AccountInfo>(a => a.AccountKind == AccountKind.Storage && a.SkuName == SkuName.StandardLrs);
        var responseMock = Mock.Of<Response<AccountInfo>>(r => r.Value == expectedAccountInfo);
        _blobServiceClientMock.Setup(x => x.GetAccountInfoAsync(default))
                              .ReturnsAsync(responseMock);

        // Act
        var accountInfo = await _blobStorageService.GetAccountInfoAsync();

        // Assert
        Assert.Equal(expectedAccountInfo, accountInfo);
    }

    [Fact]
    public async Task GetUserDelegationKeyAsync_ShouldReturnUserDelegationKey()
    {
        // Arrange
        var expectedKey = Mock.Of<UserDelegationKey>(k => k.SignedObjectId == Guid.NewGuid().ToString());
        var responseMock = Mock.Of<Response<UserDelegationKey>>(r => r.Value == expectedKey);
        _blobServiceClientMock.Setup(x => x.GetUserDelegationKeyAsync(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(), default))
                              .ReturnsAsync(responseMock);

        // Act
        var userDelegationKey = await _blobStorageService.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1));

        // Assert
        Assert.Equal(expectedKey, userDelegationKey);
    }

    private BlobGeoReplication CreateBlobGeoReplication(BlobGeoReplicationStatus status, DateTimeOffset? lastSyncedOn)
    {
        // Use reflection to create an instance of the internal class
        var constructor = typeof(BlobGeoReplication).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new Type[] { typeof(BlobGeoReplicationStatus), typeof(DateTimeOffset?) },
            null
        );

        return (BlobGeoReplication)constructor.Invoke(new object[] { status, lastSyncedOn });
    }

    private BlobServiceStatistics CreateBlobServiceStatistics(BlobGeoReplication geoReplication)
    {
        // Use reflection to create an instance of the internal class
        var constructor = typeof(BlobServiceStatistics).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new Type[] { typeof(BlobGeoReplication) },
            null
        );

        return (BlobServiceStatistics)constructor.Invoke(new object[] { geoReplication });
    }
}


