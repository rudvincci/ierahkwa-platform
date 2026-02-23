using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Moq;

namespace Mamey.Azure.Blobs.Tests;

public class BlobOperationsTests
{
    private readonly Mock<BlobServiceClient> _blobServiceClientMock;
    private readonly Mock<BlobContainerClient> _blobContainerClientMock;
    private readonly Mock<BlobClient> _blobClientMock;
    private readonly BlobStorageService _blobStorageService;

    public BlobOperationsTests()
    {
        _blobServiceClientMock = new Mock<BlobServiceClient>();
        _blobContainerClientMock = new Mock<BlobContainerClient>();
        _blobClientMock = new Mock<BlobClient>();
        _blobStorageService = new BlobStorageService(_blobServiceClientMock.Object);
    }
    [Fact]
    public async Task UploadBlockBlobAsync_ShouldUploadBlob_WhenOverwriteIsTrue()
    {
        // Arrange
        var content = new MemoryStream();
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);
        _blobClientMock.Setup(x => x.ExistsAsync(default))
            .ReturnsAsync(Response.FromValue(false, Mock.Of<Response>()));
        _blobClientMock.Setup(x => x.UploadAsync(content, true, default))
            .ReturnsAsync(Response.FromValue(Mock.Of<BlobContentInfo>(), Mock.Of<Response>()));

        // Act
        var result = await _blobStorageService.UploadBlockBlobAsync("container", "blob", content, overwrite: true);

        // Assert
        Assert.NotNull(result);
        _blobClientMock.Verify(x => x.UploadAsync(content, true, default), Times.Once);
    }

    [Fact]
    public async Task DownloadBlockBlobAsync_ShouldReturnBlobContent()
    {
        // Arrange
        var contentStream = new MemoryStream();
        var downloadDetails = BlobsModelFactory.BlobDownloadDetails(
            blobType: BlobType.Block,
            contentLength: contentStream.Length,
            contentType: "application/octet-stream",
            contentHash: null,
            lastModified: DateTimeOffset.UtcNow,
            metadata: null,
            contentRange: null,
            contentEncoding: null,
            cacheControl: null,
            contentDisposition: null,
            contentLanguage: null,
            blobSequenceNumber: default,
            copyCompletedOn: default,
            copyStatusDescription: null,
            copyId: null,
            copyProgress: null,
            copySource: null,
            copyStatus: CopyStatus.Success,
            leaseDuration: LeaseDurationType.Infinite,
            leaseState: LeaseState.Available,
            leaseStatus: LeaseStatus.Unlocked,
            acceptRanges: null,
            blobCommittedBlockCount: default,
            isServerEncrypted: false,
            encryptionKeySha256: null,
            encryptionScope: null,
            blobContentHash: null,
            tagCount: default,
            versionId: null,
            isSealed: false,
            objectReplicationSourceProperties: null,
            objectReplicationDestinationPolicy: null,
            hasLegalHold: false,
            createdOn: default
        );

        var downloadInfo = BlobsModelFactory.BlobDownloadInfo(
            content: contentStream,
            contentType: "application/octet-stream",
            contentLength: contentStream.Length,
            contentHash: null,
            blobType: BlobType.Block,
            lastModified: DateTimeOffset.UtcNow
        );

        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);
        _blobClientMock.Setup(x => x.DownloadAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response.FromValue(downloadInfo, Mock.Of<Response>()));

        // Act
        var result = await _blobStorageService.DownloadBlockBlobAsync("container", "blob");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(contentStream, result);
    }






    [Fact]
    public async Task GetBlobPropertiesAsync_ShouldReturnBlobProperties()
    {
        // Arrange
        var propertiesMock = Mock.Of<BlobProperties>();
        var responseMock = Response.FromValue(propertiesMock, Mock.Of<Response>());

        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_blobContainerClientMock.Object);

        _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);

        // Explicitly specify the optional parameters
        _blobClientMock.Setup(x => x.GetPropertiesAsync(It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseMock);

        // Act
        var result = await _blobStorageService.GetBlobPropertiesAsync("container", "blob");

        // Assert
        Assert.Equal(propertiesMock, result);
    }


    [Fact]
    public async Task SetBlobPropertiesAsync_ShouldSetBlobProperties()
    {
        // Arrange
        var headers = new BlobHttpHeaders();
        var responseMock = Mock.Of<Response<BlobInfo>>();
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);
        _blobClientMock.Setup(x => x.SetHttpHeadersAsync(headers, null, default))
            .ReturnsAsync(responseMock);

        // Act
        var result = await _blobStorageService.SetBlobPropertiesAsync("container", "blob", headers);

        // Assert
        Assert.Equal(responseMock, result);
    }

    [Fact]
    public async Task GetBlobTagsAsync_ShouldReturnBlobTags()
    {
        // Arrange
        var tags = new Dictionary<string, string> { { "key", "value" } };
        var getBlobTagResultMock = Mock.Of<GetBlobTagResult>(x => x.Tags == tags);
        var responseMock = Response.FromValue(getBlobTagResultMock, Mock.Of<Response>());
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);
        _blobClientMock.Setup(x => x.GetTagsAsync(null, default))
            .ReturnsAsync(responseMock);

        // Act
        var result = await _blobStorageService.GetBlobTagsAsync("container", "blob");

        // Assert
        Assert.Equal(tags, result);
    }

    [Fact]
    public async Task SetBlobTagsAsync_ShouldSetBlobTags()
    {
        // Arrange
        var tags = new Dictionary<string, string> { { "key", "value" } };
        var responseMock = Mock.Of<Response>();

        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_blobContainerClientMock.Object);

        _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);

        // Explicitly specify the optional parameters
        _blobClientMock.Setup(x => x.SetTagsAsync(tags, It.IsAny<BlobRequestConditions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseMock);

        // Act
        var result = await _blobStorageService.SetBlobTagsAsync("container", "blob", tags);

        // Assert
        Assert.Equal(responseMock, result);
    }


    [Fact]
    public async Task FindBlobsByTagsAsync_ShouldReturnBlobNames()
    {
        // Arrange
        var blobs = new List<string> { "blob1", "blob2" };
        var blobItems = blobs.Select(name => Mock.Of<TaggedBlobItem>(b => b.BlobName == name));
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.FindBlobsByTagsAsync(It.IsAny<string>(), default))
            .Returns(new MockAsyncPageable<TaggedBlobItem>(blobItems.ToList()));

        // Act
        var result = await _blobStorageService.FindBlobsByTagsAsync("container", "tag = 'value'");

        // Assert
        Assert.Equal(blobs, result);
    }

    [Fact]
    public async Task GetBlobMetadataAsync_ShouldReturnBlobMetadata()
    {
        // Arrange
        var metadata = new Dictionary<string, string> { { "key", "value" } };
        var properties = BlobsModelFactory.BlobProperties(
            lastModified: DateTimeOffset.UtcNow,
            metadata: metadata
            // Set other required properties as needed
        );
        var propertiesResponse = Response.FromValue(properties, Mock.Of<Response>());

        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);
        _blobClientMock.Setup(x => x.GetPropertiesAsync(null, default))
            .ReturnsAsync(propertiesResponse);

        // Act
        var result = await _blobStorageService.GetBlobMetadataAsync("container", "blob");

        // Assert
        Assert.Equal(metadata, result);
    }






    [Fact]
    public async Task SetBlobMetadataAsync_ShouldSetBlobMetadata()
    {
        // Arrange
        var metadata = new Dictionary<string, string> { { "key", "value" } };
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);

        // Mock the SetMetadataAsync method to return a Response<BlobInfo>
        var responseMock = Mock.Of<Response<BlobInfo>>();
        _blobClientMock.Setup(x => x.SetMetadataAsync(metadata, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(responseMock);

        // Act
        await _blobStorageService.SetBlobMetadataAsync("container", "blob", metadata);

        // Assert
        _blobClientMock.Verify(x => x.SetMetadataAsync(metadata, null, It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task LeaseBlobAsync_ShouldAcquireLease()
    {
        // Arrange
        var leaseId = Guid.NewGuid().ToString();
        var lease = BlobsModelFactory.BlobLease(
            leaseId: leaseId,
            eTag: new ETag(),
            lastModified: DateTimeOffset.UtcNow
        );

        var leaseResponseMock = Response.FromValue(lease, Mock.Of<Response>());
        var leaseClientMock = new Mock<BlobLeaseClient>(_blobClientMock.Object);

        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);

        _blobClientMock.Setup(x => x.GetBlobLeaseClient(It.IsAny<string>()))
            .Returns(leaseClientMock.Object);

        leaseClientMock.Setup(x => x.AcquireAsync(It.IsAny<TimeSpan>(), It.IsAny<RequestConditions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(leaseResponseMock);

        // Act
        var result = await _blobStorageService.LeaseBlobAsync("container", "blob", TimeSpan.FromSeconds(30));

        // Assert
        Assert.Equal(leaseId, result);
    }



    [Fact]
    public async Task SnapshotBlobAsync_ShouldCreateSnapshot()
    {
        // Arrange
        var snapshot = "snapshot";
        var snapshotInfo = BlobsModelFactory.BlobSnapshotInfo(snapshot, new ETag(), DateTimeOffset.UtcNow, null, true);
        var snapshotResponse = Response.FromValue(snapshotInfo, Mock.Of<Response>());

        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);
        _blobClientMock.Setup(x => x.CreateSnapshotAsync(null, null, default))
            .ReturnsAsync(snapshotResponse);
        _blobClientMock.Setup(x => x.Uri)
            .Returns(new Uri("https://example.com/container/blob")); // Ensure Uri is not null

        // Act
        var result = await _blobStorageService.SnapshotBlobAsync("container", "blob");

        // Assert
        Assert.NotNull(result);
        Assert.Contains("snapshot", result.Uri.Query);
    }


    [Fact]
    public async Task DeleteBlobAsync_ShouldDeleteBlob()
    {
        // Arrange
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);
        _blobClientMock.Setup(x => x.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Mock.Of<Response<bool>>());

        // Act
        await _blobStorageService.DeleteBlobAsync("container", "blob");

        // Assert
        _blobClientMock.Verify(x => x.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, null, It.IsAny<CancellationToken>()), Times.Once);
    }


    //[Fact]
    //public async Task DeleteBlobWithConditionAsync_ShouldDeleteBlobWithCondition()
    //{
    //    // Arrange
    //    var conditions = new BlobRequestConditions { IfModifiedSince = DateTimeOffset.UtcNow.AddMinutes(-5) };
    //    _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
    //        .Returns(_blobContainerClientMock.Object);
    //    _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
    //        .Returns(_blobClientMock.Object);
    //    _blobClientMock.Setup(x => x.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, conditions, It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(Mock.Of<Response<bool>>());

    //    // Act
    //    await _blobStorageService.DeleteBlobWithConditionAsync("container", "blob", conditions);

    //    // Assert
    //    _blobClientMock.Verify(x => x.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, conditions, It.IsAny<CancellationToken>()), Times.Once);
    //}


    [Fact]
    public async Task UndeleteBlobAsync_ShouldUndeleteBlob()
    {
        // Arrange
        _blobServiceClientMock.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
            .Returns(_blobContainerClientMock.Object);
        _blobContainerClientMock.Setup(x => x.GetBlobClient(It.IsAny<string>()))
            .Returns(_blobClientMock.Object);
        _blobClientMock.Setup(x => x.UndeleteAsync(default))
            .ReturnsAsync(Mock.Of<Response>());

        // Act
        await _blobStorageService.UndeleteBlobAsync("container", "blob");

        // Assert
        _blobClientMock.Verify(x => x.UndeleteAsync(default), Times.Once);
    }

}



