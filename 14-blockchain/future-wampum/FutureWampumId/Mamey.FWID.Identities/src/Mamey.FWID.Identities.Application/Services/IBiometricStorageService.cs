using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Persistence.Minio;
using Mamey.Persistence.Minio.Models.DTOs;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for storing and retrieving biometric data in MinIO.
/// </summary>
public interface IBiometricStorageService
{
    /// <summary>
    /// Uploads biometric data to MinIO.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="biometricType">The type of biometric data.</param>
    /// <param name="data">The biometric data to upload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The object name (path) where the data was stored.</returns>
    Task<string> UploadBiometricAsync(IdentityId identityId, Mamey.FWID.Identities.Domain.ValueObjects.BiometricType biometricType, byte[] data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads biometric data from MinIO.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="biometricType">The type of biometric data.</param>
    /// <param name="objectName">The object name (path) to download. If null, uses the default naming pattern.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The biometric data as bytes.</returns>
    Task<byte[]> DownloadBiometricAsync(IdentityId identityId, Mamey.FWID.Identities.Domain.ValueObjects.BiometricType biometricType, string? objectName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes biometric data from MinIO.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="biometricType">The type of biometric data.</param>
    /// <param name="objectName">The object name (path) to delete. If null, uses the default naming pattern.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteBiometricAsync(IdentityId identityId, Mamey.FWID.Identities.Domain.ValueObjects.BiometricType biometricType, string? objectName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a presigned URL for accessing biometric data.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="biometricType">The type of biometric data.</param>
    /// <param name="expirySeconds">The expiration time in seconds.</param>
    /// <param name="objectName">The object name (path). If null, uses the default naming pattern.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A presigned URL result.</returns>
    Task<PresignedUrlResult> GetBiometricPresignedUrlAsync(IdentityId identityId, Mamey.FWID.Identities.Domain.ValueObjects.BiometricType biometricType, int expirySeconds = 3600, string? objectName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets object metadata for biometric data.
    /// </summary>
    /// <param name="identityId">The identity identifier.</param>
    /// <param name="biometricType">The type of biometric data.</param>
    /// <param name="objectName">The object name (path). If null, uses the default naming pattern.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Object metadata.</returns>
    Task<ObjectMetadata> GetBiometricMetadataAsync(IdentityId identityId, Mamey.FWID.Identities.Domain.ValueObjects.BiometricType biometricType, string? objectName = null, CancellationToken cancellationToken = default);
}


