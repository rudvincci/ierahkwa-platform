using System.Text.Json;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.FWID.Identities.Infrastructure.Storage;
using Mamey.Persistence.Minio;
using Mamey.Persistence.Minio.Models.DTOs;
using Mamey.Persistence.Minio.Models.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.FWID.Identities.Infrastructure.MinIO.Services;

/// <summary>
/// Service for storing and retrieving biometric data in MinIO with encryption at rest.
/// Provides AES-256-GCM encryption, versioning, and audit logging.
/// </summary>
internal class BiometricStorageService : IBiometricStorageService
{
    private readonly IObjectService _objectService;
    private readonly IPresignedUrlService _presignedUrlService;
    private readonly IBiometricEncryptionService _encryptionService;
    private readonly IBiometricAuditService _auditService;
    private readonly MinioOptions _options;
    private readonly ILogger<BiometricStorageService> _logger;
    private const string BucketName = "fwid-biometrics";

    public BiometricStorageService(
        IObjectService objectService,
        IPresignedUrlService presignedUrlService,
        IBiometricEncryptionService encryptionService,
        IBiometricAuditService auditService,
        IOptions<MinioOptions> options,
        ILogger<BiometricStorageService> logger)
    {
        _objectService = objectService ?? throw new ArgumentNullException(nameof(objectService));
        _presignedUrlService = presignedUrlService ?? throw new ArgumentNullException(nameof(presignedUrlService));
        _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> UploadBiometricAsync(IdentityId identityId, BiometricType biometricType, byte[] data, CancellationToken cancellationToken = default)
    {
        if (identityId == null)
            throw new ArgumentNullException(nameof(identityId));
        if (data == null || data.Length == 0)
            throw new ArgumentException("Biometric data cannot be null or empty.", nameof(data));

        var version = await GetNextVersionAsync(identityId, biometricType, cancellationToken);
        var objectName = GenerateObjectName(identityId, biometricType, version);
        
        _logger.LogInformation("Uploading biometric data for IdentityId: {IdentityId}, Type: {BiometricType}, Version: {Version}, ObjectName: {ObjectName}", 
            identityId.Value, biometricType, version, objectName);

        try
        {
            // Encrypt biometric data before storage
            var encryptedData = await _encryptionService.EncryptAsync(data, cancellationToken);
            
            // Serialize encrypted data structure
            var encryptedBytes = SerializeEncryptedData(encryptedData);

            var metadata = new Dictionary<string, string>
            {
                { "identityId", identityId.Value.ToString() },
                { "biometricType", biometricType.ToString() },
                { "version", version.ToString() },
                { "encrypted", "true" },
                { "encryptionVersion", encryptedData.Version.ToString() },
                { "uploadedAt", DateTime.UtcNow.ToString("O") }
            };

            await _objectService.UploadBytesAsync(
                BucketName,
                objectName,
                encryptedBytes,
                "application/octet-stream",
                metadata,
                cancellationToken);

            // Audit log successful upload
            await _auditService.LogAccessAsync(identityId, biometricType, "Upload", objectName, success: true, cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully uploaded encrypted biometric data for IdentityId: {IdentityId}, Version: {Version}, ObjectName: {ObjectName}", 
                identityId.Value, version, objectName);

            return objectName;
        }
        catch (Exception ex)
        {
            // Audit log failed upload
            await _auditService.LogAccessAsync(identityId, biometricType, "Upload", objectName, success: false, ex.Message, cancellationToken);
            _logger.LogError(ex, "Failed to upload biometric data for IdentityId: {IdentityId}", identityId.Value);
            throw;
        }
    }

    public async Task<byte[]> DownloadBiometricAsync(IdentityId identityId, BiometricType biometricType, string? objectName = null, CancellationToken cancellationToken = default)
    {
        if (identityId == null)
            throw new ArgumentNullException(nameof(identityId));

        // If no object name provided, get the latest version
        if (string.IsNullOrEmpty(objectName))
        {
            objectName = await GetLatestObjectNameAsync(identityId, biometricType, cancellationToken);
        }

        _logger.LogInformation("Downloading biometric data for IdentityId: {IdentityId}, Type: {BiometricType}, ObjectName: {ObjectName}", 
            identityId.Value, biometricType, objectName);

        try
        {
            // Download encrypted data
            var encryptedBytes = await _objectService.DownloadAsBytesAsync(BucketName, objectName, null, cancellationToken);

            // Deserialize and decrypt
            var encryptedData = DeserializeEncryptedData(encryptedBytes);
            var data = await _encryptionService.DecryptAsync(encryptedData, cancellationToken);

            // Audit log successful download
            await _auditService.LogAccessAsync(identityId, biometricType, "Download", objectName, success: true, cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully downloaded and decrypted biometric data for IdentityId: {IdentityId}, Size: {Size} bytes", 
                identityId.Value, data.Length);

            return data;
        }
        catch (Exception ex)
        {
            // Audit log failed download
            await _auditService.LogAccessAsync(identityId, biometricType, "Download", objectName, success: false, ex.Message, cancellationToken);
            _logger.LogError(ex, "Failed to download biometric data for IdentityId: {IdentityId}", identityId.Value);
            throw;
        }
    }

    public async Task DeleteBiometricAsync(IdentityId identityId, BiometricType biometricType, string? objectName = null, CancellationToken cancellationToken = default)
    {
        if (identityId == null)
            throw new ArgumentNullException(nameof(identityId));

        // If no object name provided, delete all versions
        if (string.IsNullOrEmpty(objectName))
        {
            var objects = await ListObjectNamesAsync(identityId, biometricType, cancellationToken);
            foreach (var obj in objects)
            {
                await DeleteSingleObjectAsync(identityId, biometricType, obj, cancellationToken);
            }
            return;
        }

        await DeleteSingleObjectAsync(identityId, biometricType, objectName, cancellationToken);
    }

    private async Task DeleteSingleObjectAsync(IdentityId identityId, BiometricType biometricType, string objectName, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting biometric data for IdentityId: {IdentityId}, Type: {BiometricType}, ObjectName: {ObjectName}", 
            identityId.Value, biometricType, objectName);

        try
        {
            await _objectService.RemoveObjectAsync(BucketName, objectName, cancellationToken);

            // Audit log successful deletion
            await _auditService.LogAccessAsync(identityId, biometricType, "Delete", objectName, success: true, cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully deleted biometric data for IdentityId: {IdentityId}, ObjectName: {ObjectName}", 
                identityId.Value, objectName);
        }
        catch (Exception ex)
        {
            // Audit log failed deletion
            await _auditService.LogAccessAsync(identityId, biometricType, "Delete", objectName, success: false, ex.Message, cancellationToken);
            _logger.LogError(ex, "Failed to delete biometric data for IdentityId: {IdentityId}", identityId.Value);
            throw;
        }
    }

    public async Task<PresignedUrlResult> GetBiometricPresignedUrlAsync(IdentityId identityId, BiometricType biometricType, int expirySeconds = 3600, string? objectName = null, CancellationToken cancellationToken = default)
    {
        if (identityId == null)
            throw new ArgumentNullException(nameof(identityId));

        objectName ??= GenerateObjectName(identityId, biometricType);

        _logger.LogInformation("Generating presigned URL for IdentityId: {IdentityId}, Type: {BiometricType}, ObjectName: {ObjectName}, Expiry: {ExpirySeconds}s", 
            identityId.Value, biometricType, objectName, expirySeconds);

        var request = new PresignedUrlRequest
        {
            BucketName = BucketName,
            ObjectName = objectName,
            ExpiresInSeconds = expirySeconds
        };

        var result = await _presignedUrlService.PresignedGetObjectAsync(request, cancellationToken);

        _logger.LogInformation("Successfully generated presigned URL for IdentityId: {IdentityId}, ObjectName: {ObjectName}", 
            identityId.Value, objectName);

        return result;
    }

    public async Task<ObjectMetadata> GetBiometricMetadataAsync(IdentityId identityId, BiometricType biometricType, string? objectName = null, CancellationToken cancellationToken = default)
    {
        if (identityId == null)
            throw new ArgumentNullException(nameof(identityId));

        objectName ??= GenerateObjectName(identityId, biometricType);

        _logger.LogDebug("Getting metadata for IdentityId: {IdentityId}, Type: {BiometricType}, ObjectName: {ObjectName}", 
            identityId.Value, biometricType, objectName);

        var metadata = await _objectService.StatObjectAsync(BucketName, objectName, cancellationToken);

        _logger.LogDebug("Successfully retrieved metadata for IdentityId: {IdentityId}, ObjectName: {ObjectName}, Size: {Size} bytes", 
            identityId.Value, objectName, metadata.Size);

        return metadata;
    }

    /// <summary>
    /// Generates an object name using the pattern: {identityId}/{biometricType}/v{version}
    /// </summary>
    private static string GenerateObjectName(IdentityId identityId, BiometricType biometricType, int version)
    {
        return $"{identityId.Value}/{biometricType}/v{version}.enc";
    }

    /// <summary>
    /// Gets the next version number for a biometric type.
    /// </summary>
    private async Task<int> GetNextVersionAsync(IdentityId identityId, BiometricType biometricType, CancellationToken cancellationToken)
    {
        var objects = await ListObjectNamesAsync(identityId, biometricType, cancellationToken);
        if (!objects.Any())
        {
            return 1;
        }

        // Extract version numbers from object names
        var versions = objects
            .Select(obj => ExtractVersionFromObjectName(obj))
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .ToList();

        return versions.Any() ? versions.Max() + 1 : 1;
    }

    /// <summary>
    /// Gets the latest object name for a biometric type.
    /// </summary>
    private async Task<string> GetLatestObjectNameAsync(IdentityId identityId, BiometricType biometricType, CancellationToken cancellationToken)
    {
        var objects = await ListObjectNamesAsync(identityId, biometricType, cancellationToken);
        if (!objects.Any())
        {
            throw new InvalidOperationException($"No biometric data found for IdentityId: {identityId.Value}, Type: {biometricType}");
        }

        // Get the object with the highest version
        var latestObject = objects
            .OrderByDescending(obj => ExtractVersionFromObjectName(obj) ?? 0)
            .First();

        return latestObject;
    }

    /// <summary>
    /// Lists all object names for a given identity and biometric type.
    /// </summary>
    private async Task<List<string>> ListObjectNamesAsync(IdentityId identityId, BiometricType biometricType, CancellationToken cancellationToken)
    {
        var prefix = $"{identityId.Value}/{biometricType}/";
        // TODO: Implement list objects with prefix using IObjectService
        // For now, return empty list - this would need to be implemented in the MinIO service
        return new List<string>();
    }

    /// <summary>
    /// Extracts version number from object name (e.g., "v5.enc" -> 5).
    /// </summary>
    private static int? ExtractVersionFromObjectName(string objectName)
    {
        var fileName = Path.GetFileNameWithoutExtension(objectName);
        if (fileName.StartsWith("v") && int.TryParse(fileName.Substring(1), out var version))
        {
            return version;
        }
        return null;
    }

    /// <summary>
    /// Serializes encrypted biometric data to bytes.
    /// </summary>
    private static byte[] SerializeEncryptedData(EncryptedBiometricData encryptedData)
    {
        var json = JsonSerializer.Serialize(new
        {
            ciphertext = Convert.ToBase64String(encryptedData.Ciphertext),
            iv = Convert.ToBase64String(encryptedData.IV),
            tag = Convert.ToBase64String(encryptedData.Tag),
            version = encryptedData.Version
        });
        return System.Text.Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// Deserializes encrypted biometric data from bytes.
    /// </summary>
    private static EncryptedBiometricData DeserializeEncryptedData(byte[] data)
    {
        var json = System.Text.Encoding.UTF8.GetString(data);
        var obj = JsonSerializer.Deserialize<JsonElement>(json);
        
        return new EncryptedBiometricData
        {
            Ciphertext = Convert.FromBase64String(obj.GetProperty("ciphertext").GetString()!),
            IV = Convert.FromBase64String(obj.GetProperty("iv").GetString()!),
            Tag = Convert.FromBase64String(obj.GetProperty("tag").GetString()!),
            Version = obj.GetProperty("version").GetInt32()
        };
    }
}


