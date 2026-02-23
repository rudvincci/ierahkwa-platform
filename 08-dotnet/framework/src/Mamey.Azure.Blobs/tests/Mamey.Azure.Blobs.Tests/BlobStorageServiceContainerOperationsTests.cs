using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Moq;

namespace Mamey.Azure.Blobs.Tests;

public class BlobStorageServiceContainerOperationsTests
{
    private readonly Mock<BlobServiceClient> _blobServiceClientMock;
    private readonly Mock<BlobContainerClient> _blobContainerClientMock;
    private readonly BlobStorageService _blobStorageService;

    public BlobStorageServiceContainerOperationsTests()
    {
        _blobServiceClientMock = new Mock<BlobServiceClient>();
        _blobContainerClientMock = new Mock<BlobContainerClient>();
        _blobStorageService = new BlobStorageService(_blobServiceClientMock.Object);
    }

    [Fact]
    public async Task CreateContainerAsync_ShouldCreateContainer()
    {
        // Arrange
        var mockResponse = Mock.Of<Response<BlobContainerInfo>>();
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                              .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.CreateAsync(PublicAccessType.None, null, null, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(mockResponse);

        // Act
        var containerClient = await _blobStorageService.CreateContainerAsync("test-container");

        // Assert
        _blobContainerClientMock.Verify(x => x.CreateAsync(PublicAccessType.None, null, null, It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(_blobContainerClientMock.Object, containerClient);
    }

    [Fact]
    public async Task GetContainerPropertiesAsync_ShouldReturnContainerProperties()
    {
        // Arrange
        var propertiesMock = Mock.Of<BlobContainerProperties>();
        var responseMock = Mock.Of<Response<BlobContainerProperties>>(r => r.Value == propertiesMock);
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                              .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
                                .ReturnsAsync(responseMock);

        // Act
        var properties = await _blobStorageService.GetContainerPropertiesAsync("test-container");

        // Assert
        Assert.Equal(propertiesMock, properties);
    }

    [Fact]
    public async Task GetContainerMetadataAsync_ShouldReturnContainerMetadata()
    {
        // Arrange
        var metadata = new Dictionary<string, string> { { "key1", "value1" } };
        var propertiesMock = Mock.Of<BlobContainerProperties>(p => p.Metadata == metadata);
        var responseMock = Mock.Of<Response<BlobContainerProperties>>(r => r.Value == propertiesMock);
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                              .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
                                .ReturnsAsync(responseMock);

        // Act
        var result = await _blobStorageService.GetContainerMetadataAsync("test-container");

        // Assert
        Assert.Equal(metadata, result);
    }

    [Fact]
    public async Task SetContainerMetadataAsync_ShouldSetContainerMetadata()
    {
        /// Arrange
        var metadata = new Dictionary<string, string> { { "key1", "value1" } };
        var mockResponse = Mock.Of<Response<BlobContainerInfo>>(); // Correct the return type to Response<BlobContainerInfo>
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                              .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.SetMetadataAsync(metadata, It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
                                .ReturnsAsync(mockResponse);

        // Act
        await _blobStorageService.SetContainerMetadataAsync("test-container", metadata);

        // Assert
        _blobContainerClientMock.Verify(x => x.SetMetadataAsync(metadata, It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetContainerAclAsync_ShouldReturnContainerAcl()
    {
        // Arrange
        var aclMock = Mock.Of<BlobContainerAccessPolicy>();
        var responseMock = Mock.Of<Response<BlobContainerAccessPolicy>>(r => r.Value == aclMock);
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                              .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.GetAccessPolicyAsync(null, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(responseMock);

        // Act
        var result = await _blobStorageService.GetContainerAclAsync("test-container");

        // Assert
        Assert.Equal(aclMock, result);
    }

    [Fact]
    public async Task SetContainerAclAsync_ShouldSetContainerAcl()
    {
        // Arrange
        var permissions = new List<BlobSignedIdentifier>();
        var mockResponse = Mock.Of<Response<BlobContainerInfo>>(); // Use Response<BlobContainerInfo> as the return type
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                              .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.SetAccessPolicyAsync(It.IsAny<PublicAccessType>(), permissions, It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
                                .ReturnsAsync(mockResponse);

        // Act
        await _blobStorageService.SetContainerAclAsync("test-container", PublicAccessType.Blob, permissions);

        // Assert
        _blobContainerClientMock.Verify(x => x.SetAccessPolicyAsync(PublicAccessType.Blob, permissions, It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteContainerAsync_ShouldDeleteContainer()
    {
        // Arrange
        var mockResponse = Mock.Of<Response>();
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                              .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.DeleteAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
                                .ReturnsAsync(mockResponse);

        // Act
        await _blobStorageService.DeleteContainerAsync("test-container");

        // Assert
        _blobContainerClientMock.Verify(x => x.DeleteAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LeaseContainerAsync_ShouldLeaseContainer()
    {
        // Arrange
        var leaseId = Guid.NewGuid().ToString();
        var leaseResponse = Mock.Of<Response<BlobLease>>(r => r.Value.LeaseId == leaseId);
        var leaseClientMock = new Mock<BlobLeaseClient>();

        // Set up the lease client to return the mocked response when AcquireAsync is called
        leaseClientMock.Setup(x => x.AcquireAsync(It.IsAny<TimeSpan>(), It.IsAny<RequestConditions>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(leaseResponse);

        // Set up the BlobStorageService1 to return the mocked lease client when GetLeaseClient is called
        var blobStorageServiceMock = new Mock<BlobStorageService>(_blobServiceClientMock.Object);
        blobStorageServiceMock.Setup(x => x.GetLeaseClient(_blobContainerClientMock.Object, null))
                              .Returns(leaseClientMock.Object);

        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                              .Returns(_blobContainerClientMock.Object);

        // Act
        var result = await blobStorageServiceMock.Object.LeaseContainerAsync("test-container", leaseClientMock.Object, TimeSpan.FromSeconds(30));

        // Assert
        Assert.Equal(leaseId, result);
        leaseClientMock.Verify(x => x.AcquireAsync(It.IsAny<TimeSpan>(), It.IsAny<RequestConditions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListBlobsAsync_ShouldReturnListOfBlobs()
    {
        // Arrange
        var blobNames = new List<string> { "blob1", "blob2" };
        var blobs = new List<BlobItem>
        {
            Mock.Of<BlobItem>(b => b.Name == "blob1"),
            Mock.Of<BlobItem>(b => b.Name == "blob2")
        };

        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                              .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.GetBlobsAsync(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), null, It.IsAny<CancellationToken>()))
                                .Returns(new MockAsyncPageable<BlobItem>(blobs.ToList()));

        // Act
        var result = await _blobStorageService.ListBlobsAsync("test-container");

        // Assert
        Assert.Equal(blobs, result);
    }
    [Fact]
    public async Task ListBlobNamesAsync_ShouldReturnListOfBlobNamess()
    {
        // Arrange
        var blobNames = new List<string> { "blob1", "blob2" };
        var blobs = new List<BlobItem>
        {
            Mock.Of<BlobItem>(b => b.Name == "blob1"),
            Mock.Of<BlobItem>(b => b.Name == "blob2")
        };

        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                              .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.GetBlobsAsync(It.IsAny<BlobTraits>(), It.IsAny<BlobStates>(), null, It.IsAny<CancellationToken>()))
                                .Returns(new MockAsyncPageable<BlobItem>(blobs.ToList()));

        // Act
        var result = await _blobStorageService.ListBlobNamesAsync("test-container");

        // Assert
        Assert.Equal(blobNames, result);
    }

    [Fact]
    public async Task RestoreContainerAsync_ShouldRestoreContainer()
    {
        // Arrange
        var mockResponse = Mock.Of<Response<BlobContainerClient>>(); // Correct the return type to Response<BlobContainerClient>
        _blobServiceClientMock.Setup(x => x.UndeleteBlobContainerAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                              .ReturnsAsync(mockResponse);

        // Act
        await _blobStorageService.RestoreContainerAsync("deleted-container", "restored-container");

        // Assert
        _blobServiceClientMock.Verify(x => x.UndeleteBlobContainerAsync("deleted-container", "restored-container", It.IsAny<CancellationToken>()), Times.Once);
    }
}



