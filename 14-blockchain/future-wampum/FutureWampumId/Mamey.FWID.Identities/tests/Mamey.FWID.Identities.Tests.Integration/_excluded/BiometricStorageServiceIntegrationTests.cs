using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Infrastructure.MinIO.Services;
using Mamey.FWID.Identities.Tests.Shared.Factories;
using Mamey.FWID.Identities.Tests.Shared.Fixtures;
using Mamey.Persistence.Minio;
using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;
using Mamey.Types;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Integration.Services;

/// <summary>
/// Integration tests for BiometricStorageService with real MinIO.
/// </summary>
[Collection("Integration")]
public class BiometricStorageServiceIntegrationTests : IClassFixture<MinIOFixture>, IAsyncLifetime
{
    private readonly MinIOFixture _fixture;
    private IServiceProvider? _serviceProvider;
    private IBiometricStorageService? _service;

    public BiometricStorageServiceIntegrationTests(MinIOFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
    {
        // Configure services
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));

        // Add MinIO services
        services.AddSingleton(_fixture.ObjectService);
        services.AddSingleton(_fixture.PresignedUrlService);
        services.AddSingleton(_fixture.BucketService);

        // Add MinIO options
        var minioOptions = new MinioOptions
        {
            Endpoint = _fixture.Endpoint,
            AccessKey = _fixture.AccessKey,
            SecretKey = _fixture.SecretKey,
            Bucket = "biometric-data",
            UseSSL = false
        };
        services.AddSingleton(Options.Create(minioOptions));

        // Add BiometricStorageService
        services.AddScoped<IBiometricStorageService, BiometricStorageService>();

        _serviceProvider = services.BuildServiceProvider();

        // Ensure bucket exists
        var bucketExists = await _fixture.BucketService.BucketExistsAsync("biometric-data", default);
        if (!bucketExists)
        {
            await _fixture.BucketService.MakeBucketAsync("biometric-data", default);
        }

        // Get service
        _service = _serviceProvider.GetRequiredService<IBiometricStorageService>();
    }

    public async Task DisposeAsync()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    [Fact]
    public async Task UploadBiometricAsync_ShouldStoreBiometricData()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricType = BiometricType.Fingerprint;
        var data = TestDataFactory.CreateTestBiometricData(biometricType, 1024).EncryptedTemplate;

        // Act
        var objectName = await _service!.UploadBiometricAsync(identityId, biometricType, data);

        // Assert
        objectName.ShouldNotBeNullOrEmpty();
        objectName.ShouldContain(identityId.Value.ToString());
        objectName.ShouldContain(biometricType.ToString());
    }

    [Fact]
    public async Task DownloadBiometricAsync_ShouldRetrieveBiometricData()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricType = BiometricType.Facial;
        var originalData = TestDataFactory.CreateTestBiometricData(biometricType, 2048).EncryptedTemplate;
        var objectName = await _service!.UploadBiometricAsync(identityId, biometricType, originalData);

        // Act
        var downloadedData = await _service.DownloadBiometricAsync(identityId, biometricType, objectName);

        // Assert
        downloadedData.ShouldNotBeNull();
        downloadedData.Length.ShouldBe(originalData.Length);
        downloadedData.ShouldBe(originalData);
    }

    [Fact]
    public async Task DeleteBiometricAsync_ShouldRemoveBiometricData()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricType = BiometricType.Fingerprint;
        var data = TestDataFactory.CreateTestBiometricData(biometricType, 1024).EncryptedTemplate;
        var objectName = await _service!.UploadBiometricAsync(identityId, biometricType, data);

        // Act
        await _service.DeleteBiometricAsync(identityId, biometricType, objectName);

        // Assert - Should throw exception when trying to download deleted data
        await Should.ThrowAsync<Exception>(async () =>
            await _service.DownloadBiometricAsync(identityId, biometricType, objectName));
    }

    [Fact]
    public async Task GetBiometricPresignedUrlAsync_ShouldReturnPresignedUrl()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricType = BiometricType.Fingerprint;
        var data = TestDataFactory.CreateTestBiometricData(biometricType, 1024).EncryptedTemplate;
        var objectName = await _service!.UploadBiometricAsync(identityId, biometricType, data);

        // Act
        var presignedUrl = await _service.GetBiometricPresignedUrlAsync(identityId, biometricType, 3600, objectName);

        // Assert
        presignedUrl.ShouldNotBeNull();
        presignedUrl.Url.ShouldNotBeNullOrEmpty();
        presignedUrl.Expiration.ShouldBeGreaterThan(DateTime.UtcNow);
    }

    [Fact]
    public async Task GetBiometricMetadataAsync_ShouldReturnMetadata()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricType = BiometricType.Facial;
        var data = TestDataFactory.CreateTestBiometricData(biometricType, 2048).EncryptedTemplate;
        var objectName = await _service!.UploadBiometricAsync(identityId, biometricType, data);

        // Act
        var metadata = await _service.GetBiometricMetadataAsync(identityId, biometricType, objectName);

        // Assert
        metadata.ShouldNotBeNull();
        metadata.Size.ShouldBe(data.Length);
        metadata.Name.ShouldBe(objectName);
    }

    [Fact]
    public async Task UploadBiometricAsync_WithNullData_ShouldThrowException()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricType = BiometricType.Fingerprint;

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _service!.UploadBiometricAsync(identityId, biometricType, null!));
    }

    [Fact]
    public async Task UploadBiometricAsync_WithEmptyData_ShouldThrowException()
    {
        // Arrange
        var identityId = new IdentityId(Guid.NewGuid());
        var biometricType = BiometricType.Fingerprint;
        var emptyData = Array.Empty<byte>();

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _service!.UploadBiometricAsync(identityId, biometricType, emptyData));
    }
}

