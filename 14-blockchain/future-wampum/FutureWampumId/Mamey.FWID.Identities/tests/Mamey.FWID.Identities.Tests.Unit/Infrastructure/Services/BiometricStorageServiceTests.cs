using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Infrastructure.MinIO.Services;
using Mamey.Persistence.Minio;
using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Infrastructure.Services;

public class BiometricStorageServiceTests
{
    private readonly IObjectService _objectService;
    private readonly IPresignedUrlService _presignedUrlService;
    private readonly IOptions<MinioOptions> _options;
    private readonly ILogger<BiometricStorageService> _logger;
    private readonly BiometricStorageService _service;

    public BiometricStorageServiceTests()
    {
        _objectService = Substitute.For<IObjectService>();
        _presignedUrlService = Substitute.For<IPresignedUrlService>();
        _options = Substitute.For<IOptions<MinioOptions>>();
        _options.Value.Returns(new MinioOptions());
        _logger = Substitute.For<ILogger<BiometricStorageService>>();
        _service = new BiometricStorageService(_objectService, _presignedUrlService, _options, _logger);
    }

    [Fact]
    public async Task UploadBiometricAsync_WithValidData_ShouldUploadToMinIO()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricType = BiometricType.Fingerprint;
        var data = new byte[] { 1, 2, 3 };

        var objectMetadata = new ObjectMetadata
        {
            Name = $"biometric-data/{identityId.Value}/{biometricType.ToString().ToLowerInvariant()}.bin",
            Size = data.Length,
            LastModified = DateTime.UtcNow,
            ETag = "test-etag",
            ContentType = "application/octet-stream"
        };
        
        _objectService.UploadBytesAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<byte[]>(),
            Arg.Any<string>(),
            Arg.Any<Dictionary<string, string>>(),
            Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(objectMetadata));

        // Act
        var result = await _service.UploadBiometricAsync(identityId, biometricType, data);

        // Assert
        result.ShouldNotBeNullOrEmpty();
        result.ShouldContain(identityId.Value.ToString());
        result.ShouldContain(biometricType.ToString());
        await _objectService.Received(1).UploadBytesAsync(
            Arg.Is<string>(b => b == "biometric-data"),
            Arg.Any<string>(),
            Arg.Is<byte[]>(d => d.SequenceEqual(data)),
            Arg.Is<string>(c => c == "application/octet-stream"),
            Arg.Any<Dictionary<string, string>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UploadBiometricAsync_WithNullIdentityId_ShouldThrowException()
    {
        // Arrange
        IdentityId? identityId = null;
        var biometricType = BiometricType.Fingerprint;
        var data = new byte[] { 1, 2, 3 };

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(
            () => _service.UploadBiometricAsync(identityId!, biometricType, data));
    }

    [Fact]
    public async Task UploadBiometricAsync_WithEmptyData_ShouldThrowException()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricType = BiometricType.Fingerprint;
        var data = Array.Empty<byte>(); // Empty data should throw exception

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(
            () => _service.UploadBiometricAsync(identityId, biometricType, data));
    }

    [Fact]
    public async Task DownloadBiometricAsync_WithValidIdentityId_ShouldDownloadFromMinIO()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricType = BiometricType.Fingerprint;
        var expectedData = new byte[] { 1, 2, 3 };

        _objectService.DownloadAsBytesAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<IProgress<long>?>(),
            Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expectedData));

        // Act
        var result = await _service.DownloadBiometricAsync(identityId, biometricType);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBe(expectedData);
        await _objectService.Received(1).DownloadAsBytesAsync(
            Arg.Is<string>(b => b == "biometric-data"),
            Arg.Any<string>(),
            Arg.Any<IProgress<long>?>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DownloadBiometricAsync_WithNullIdentityId_ShouldThrowException()
    {
        // Arrange
        IdentityId? identityId = null;
        var biometricType = BiometricType.Fingerprint;

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(
            () => _service.DownloadBiometricAsync(identityId!, biometricType));
    }

    [Fact]
    public async Task DeleteBiometricAsync_WithValidIdentityId_ShouldDeleteFromMinIO()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricType = BiometricType.Fingerprint;

        _objectService.RemoveObjectAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteBiometricAsync(identityId, biometricType);

        // Assert
        await _objectService.Received(1).RemoveObjectAsync(
            Arg.Is<string>(b => b == "biometric-data"),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetBiometricPresignedUrlAsync_WithValidIdentityId_ShouldReturnPresignedUrl()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricType = BiometricType.Fingerprint;
        var expirySeconds = 3600;
        var expectedResult = new PresignedUrlResult
        {
            Url = "https://minio.example.com/presigned-url",
            Expiration = DateTime.UtcNow.AddSeconds(expirySeconds)
        };

        _presignedUrlService.PresignedGetObjectAsync(
            Arg.Any<PresignedUrlRequest>(),
            Arg.Any<CancellationToken>())
            .Returns(expectedResult);

        // Act
        var result = await _service.GetBiometricPresignedUrlAsync(identityId, biometricType, expirySeconds);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldBe(expectedResult.Url);
        await _presignedUrlService.Received(1).PresignedGetObjectAsync(
            Arg.Is<PresignedUrlRequest>(r => 
                r.BucketName == "biometric-data" &&
                r.ExpiresInSeconds == expirySeconds),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetBiometricMetadataAsync_WithValidIdentityId_ShouldReturnMetadata()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricType = BiometricType.Fingerprint;
        var expectedMetadata = new ObjectMetadata
        {
            Name = "test-object",
            Size = 1024,
            ContentType = "application/octet-stream"
        };

        _objectService.StatObjectAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>())
            .Returns(expectedMetadata);

        // Act
        var result = await _service.GetBiometricMetadataAsync(identityId, biometricType);

        // Assert
        result.ShouldNotBeNull();
        result.Size.ShouldBe(expectedMetadata.Size);
        await _objectService.Received(1).StatObjectAsync(
            Arg.Is<string>(b => b == "biometric-data"),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>());
    }
}

