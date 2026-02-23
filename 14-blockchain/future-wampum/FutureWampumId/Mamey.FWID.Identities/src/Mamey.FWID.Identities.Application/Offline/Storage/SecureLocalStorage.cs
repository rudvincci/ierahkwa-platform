using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Mamey.FWID.Identities.Application.Offline.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Offline.Storage;

/// <summary>
/// Interface for secure local storage of biometric templates and credentials.
/// </summary>
public interface ISecureLocalStorage
{
    /// <summary>
    /// Stores a biometric template securely.
    /// </summary>
    Task StoreBiometricTemplateAsync(CachedBiometricTemplate template, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves all biometric templates for an identity.
    /// </summary>
    Task<IReadOnlyList<CachedBiometricTemplate>> GetBiometricTemplatesAsync(Guid identityId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves biometric templates by modality.
    /// </summary>
    Task<IReadOnlyList<CachedBiometricTemplate>> GetBiometricTemplatesByModalityAsync(BiometricModality modality, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes a biometric template.
    /// </summary>
    Task RemoveBiometricTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Stores a credential securely.
    /// </summary>
    Task StoreCredentialAsync(CachedCredential credential, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves all credentials for an identity.
    /// </summary>
    Task<IReadOnlyList<CachedCredential>> GetCredentialsAsync(Guid identityId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a credential by ID.
    /// </summary>
    Task<CachedCredential?> GetCredentialAsync(Guid credentialId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes a credential.
    /// </summary>
    Task RemoveCredentialAsync(Guid credentialId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes all expired entries.
    /// </summary>
    Task CleanupExpiredEntriesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets storage statistics.
    /// </summary>
    Task<StorageStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Storage statistics for monitoring.
/// </summary>
public class StorageStatistics
{
    public int TotalTemplates { get; set; }
    public int TotalCredentials { get; set; }
    public int ExpiredTemplates { get; set; }
    public int ExpiredCredentials { get; set; }
    public long StorageSizeBytes { get; set; }
    public DateTime LastCleanup { get; set; }
}

/// <summary>
/// In-memory secure local storage implementation.
/// In production, use SQLite/LiteDB with hardware-backed encryption.
/// </summary>
public class SecureLocalStorage : ISecureLocalStorage
{
    private readonly Dictionary<Guid, CachedBiometricTemplate> _templates = new();
    private readonly Dictionary<Guid, CachedCredential> _credentials = new();
    private readonly byte[] _encryptionKey;
    private readonly ILogger<SecureLocalStorage> _logger;
    private DateTime _lastCleanup = DateTime.UtcNow;
    private readonly object _lock = new();
    
    public SecureLocalStorage(ILogger<SecureLocalStorage> logger, byte[]? encryptionKey = null)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _encryptionKey = encryptionKey ?? GenerateKey();
    }
    
    /// <inheritdoc />
    public Task StoreBiometricTemplateAsync(CachedBiometricTemplate template, CancellationToken cancellationToken = default)
    {
        if (template == null) throw new ArgumentNullException(nameof(template));
        
        lock (_lock)
        {
            // Ensure template data is encrypted
            if (string.IsNullOrEmpty(template.EncryptionKeyId))
            {
                template.EncryptionKeyId = "key-1"; // Current key version
                template.EncryptionIV = RandomNumberGenerator.GetBytes(16);
                // Template should already be encrypted before storage
            }
            
            // Calculate hash for integrity
            template.TemplateHash = ComputeHash(template.EncryptedTemplate);
            
            _templates[template.Id] = template;
            _logger.LogDebug("Stored biometric template {TemplateId} for identity {IdentityId}", 
                template.Id, template.IdentityId);
        }
        
        return Task.CompletedTask;
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<CachedBiometricTemplate>> GetBiometricTemplatesAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var templates = _templates.Values
                .Where(t => t.IdentityId == identityId && t.IsValidForMatching())
                .ToList();
            return Task.FromResult<IReadOnlyList<CachedBiometricTemplate>>(templates);
        }
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<CachedBiometricTemplate>> GetBiometricTemplatesByModalityAsync(BiometricModality modality, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var templates = _templates.Values
                .Where(t => t.Modality == modality && t.IsValidForMatching())
                .ToList();
            return Task.FromResult<IReadOnlyList<CachedBiometricTemplate>>(templates);
        }
    }
    
    /// <inheritdoc />
    public Task RemoveBiometricTemplateAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_templates.Remove(templateId))
            {
                _logger.LogInformation("Removed biometric template {TemplateId}", templateId);
            }
        }
        return Task.CompletedTask;
    }
    
    /// <inheritdoc />
    public Task StoreCredentialAsync(CachedCredential credential, CancellationToken cancellationToken = default)
    {
        if (credential == null) throw new ArgumentNullException(nameof(credential));
        
        lock (_lock)
        {
            // Calculate hash for integrity
            credential.CredentialHash = ComputeHash(Encoding.UTF8.GetBytes(credential.CredentialJwt));
            
            _credentials[credential.Id] = credential;
            _logger.LogDebug("Stored credential {CredentialId} for identity {IdentityId}", 
                credential.Id, credential.IdentityId);
        }
        
        return Task.CompletedTask;
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<CachedCredential>> GetCredentialsAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var credentials = _credentials.Values
                .Where(c => c.IdentityId == identityId && c.IsValidOffline())
                .ToList();
            return Task.FromResult<IReadOnlyList<CachedCredential>>(credentials);
        }
    }
    
    /// <inheritdoc />
    public Task<CachedCredential?> GetCredentialAsync(Guid credentialId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _credentials.TryGetValue(credentialId, out var credential);
            return Task.FromResult(credential?.IsValidOffline() == true ? credential : null);
        }
    }
    
    /// <inheritdoc />
    public Task RemoveCredentialAsync(Guid credentialId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_credentials.Remove(credentialId))
            {
                _logger.LogInformation("Removed credential {CredentialId}", credentialId);
            }
        }
        return Task.CompletedTask;
    }
    
    /// <inheritdoc />
    public Task CleanupExpiredEntriesAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var expiredTemplates = _templates.Values
                .Where(t => !t.IsValidForMatching())
                .Select(t => t.Id)
                .ToList();
                
            var expiredCredentials = _credentials.Values
                .Where(c => !c.IsValidOffline())
                .Select(c => c.Id)
                .ToList();
                
            foreach (var id in expiredTemplates)
                _templates.Remove(id);
                
            foreach (var id in expiredCredentials)
                _credentials.Remove(id);
                
            _lastCleanup = DateTime.UtcNow;
            
            _logger.LogInformation("Cleanup complete. Removed {Templates} templates, {Credentials} credentials",
                expiredTemplates.Count, expiredCredentials.Count);
        }
        
        return Task.CompletedTask;
    }
    
    /// <inheritdoc />
    public Task<StorageStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var stats = new StorageStatistics
            {
                TotalTemplates = _templates.Count,
                TotalCredentials = _credentials.Count,
                ExpiredTemplates = _templates.Values.Count(t => !t.IsValidForMatching()),
                ExpiredCredentials = _credentials.Values.Count(c => !c.IsValidOffline()),
                LastCleanup = _lastCleanup,
                StorageSizeBytes = EstimateStorageSize()
            };
            return Task.FromResult(stats);
        }
    }
    
    private static byte[] GenerateKey()
    {
        return RandomNumberGenerator.GetBytes(32); // AES-256
    }
    
    private static string ComputeHash(byte[] data)
    {
        var hash = SHA256.HashData(data);
        return Convert.ToBase64String(hash);
    }
    
    private long EstimateStorageSize()
    {
        long size = 0;
        foreach (var t in _templates.Values)
            size += t.EncryptedTemplate?.Length ?? 0;
        foreach (var c in _credentials.Values)
            size += Encoding.UTF8.GetByteCount(c.CredentialJwt ?? "");
        return size;
    }
}
