using System.Security.Cryptography;
using Mamey.FWID.Identities.Application.Offline.Models;
using Mamey.FWID.Identities.Application.Offline.Storage;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Offline.Services;

/// <summary>
/// Interface for offline biometric verification service.
/// </summary>
public interface IOfflineBiometricService
{
    /// <summary>
    /// Verifies a biometric sample against cached templates.
    /// </summary>
    Task<OfflineVerificationResult> VerifyAsync(
        byte[] biometricSample,
        BiometricModality modality,
        OfflineVerificationOptions? options = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Verifies a biometric sample against a specific identity.
    /// </summary>
    Task<OfflineVerificationResult> VerifyIdentityAsync(
        Guid identityId,
        byte[] biometricSample,
        BiometricModality modality,
        OfflineVerificationOptions? options = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Caches a biometric template for offline verification.
    /// </summary>
    Task CacheTemplateAsync(
        Guid identityId,
        byte[] templateData,
        BiometricModality modality,
        double qualityScore,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes cached templates for an identity.
    /// </summary>
    Task RemoveCachedTemplatesAsync(Guid identityId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if offline verification is available for a modality.
    /// </summary>
    Task<bool> IsAvailableAsync(BiometricModality modality, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets pending verification records for sync.
    /// </summary>
    Task<IReadOnlyList<OfflineVerificationResult>> GetPendingSyncResultsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Marks verification results as synced.
    /// </summary>
    Task MarkSyncedAsync(IEnumerable<string> transactionIds, CancellationToken cancellationToken = default);
}

/// <summary>
/// Options for offline biometric verification.
/// </summary>
public class OfflineVerificationOptions
{
    /// <summary>
    /// Minimum match score required (0.0 to 1.0).
    /// </summary>
    public double MinMatchScore { get; set; } = 0.8;
    
    /// <summary>
    /// Minimum quality score required (0.0 to 1.0).
    /// </summary>
    public double MinQualityScore { get; set; } = 0.6;
    
    /// <summary>
    /// Maximum number of templates to match against.
    /// </summary>
    public int MaxTemplates { get; set; } = 100;
    
    /// <summary>
    /// Device identifier for audit.
    /// </summary>
    public string? DeviceId { get; set; }
    
    /// <summary>
    /// Whether to require cached credential validation.
    /// </summary>
    public bool RequireValidCredential { get; set; } = true;
}

/// <summary>
/// Offline biometric verification service implementation.
/// Performs local biometric matching against cached templates.
/// </summary>
public class OfflineBiometricService : IOfflineBiometricService
{
    private readonly ISecureLocalStorage _storage;
    private readonly ILogger<OfflineBiometricService> _logger;
    private readonly List<OfflineVerificationResult> _pendingSync = new();
    private readonly object _syncLock = new();
    
    public OfflineBiometricService(
        ISecureLocalStorage storage,
        ILogger<OfflineBiometricService> logger)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc />
    public async Task<OfflineVerificationResult> VerifyAsync(
        byte[] biometricSample,
        BiometricModality modality,
        OfflineVerificationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new OfflineVerificationOptions();
        
        try
        {
            _logger.LogInformation("Starting offline biometric verification for modality: {Modality}", modality);
            
            // Calculate quality score of the captured sample
            var qualityScore = CalculateQualityScore(biometricSample, modality);
            if (qualityScore < options.MinQualityScore)
            {
                _logger.LogWarning("Biometric quality too low: {Quality} < {MinQuality}", 
                    qualityScore, options.MinQualityScore);
                return OfflineVerificationResult.Failure(
                    "LOW_QUALITY",
                    $"Biometric quality score ({qualityScore:F2}) below threshold ({options.MinQualityScore:F2})",
                    modality);
            }
            
            // Get cached templates for this modality
            var templates = await _storage.GetBiometricTemplatesByModalityAsync(modality, cancellationToken);
            if (!templates.Any())
            {
                _logger.LogWarning("No cached templates available for modality: {Modality}", modality);
                return OfflineVerificationResult.Failure(
                    "NO_TEMPLATES",
                    "No cached biometric templates available for this modality",
                    modality);
            }
            
            // Limit templates to match against
            var templatesToMatch = templates.Take(options.MaxTemplates).ToList();
            _logger.LogDebug("Matching against {Count} templates", templatesToMatch.Count);
            
            // Perform matching
            var bestMatch = await FindBestMatchAsync(biometricSample, templatesToMatch, cancellationToken);
            
            if (bestMatch.matchScore >= options.MinMatchScore)
            {
                var result = OfflineVerificationResult.Success(
                    bestMatch.template.IdentityId,
                    modality,
                    bestMatch.matchScore,
                    qualityScore,
                    wasOffline: true);
                result.DeviceId = options.DeviceId;
                
                // Queue for sync
                AddPendingSync(result);
                
                _logger.LogInformation("Offline biometric verification successful. Identity: {IdentityId}, Score: {Score}",
                    bestMatch.template.IdentityId, bestMatch.matchScore);
                    
                return result;
            }
            
            _logger.LogInformation("Offline biometric verification: no match found above threshold");
            return OfflineVerificationResult.NoMatch(modality, bestMatch.matchScore, qualityScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during offline biometric verification");
            return OfflineVerificationResult.Failure("INTERNAL_ERROR", ex.Message, modality);
        }
    }
    
    /// <inheritdoc />
    public async Task<OfflineVerificationResult> VerifyIdentityAsync(
        Guid identityId,
        byte[] biometricSample,
        BiometricModality modality,
        OfflineVerificationOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new OfflineVerificationOptions();
        
        try
        {
            _logger.LogInformation("Starting offline identity verification for identity: {IdentityId}", identityId);
            
            // Calculate quality score
            var qualityScore = CalculateQualityScore(biometricSample, modality);
            if (qualityScore < options.MinQualityScore)
            {
                return OfflineVerificationResult.Failure(
                    "LOW_QUALITY",
                    $"Biometric quality score ({qualityScore:F2}) below threshold",
                    modality);
            }
            
            // Get templates for specific identity
            var templates = await _storage.GetBiometricTemplatesAsync(identityId, cancellationToken);
            var modalityTemplates = templates.Where(t => t.Modality == modality).ToList();
            
            if (!modalityTemplates.Any())
            {
                return OfflineVerificationResult.Failure(
                    "NO_TEMPLATES",
                    $"No cached templates for identity {identityId} with modality {modality}",
                    modality);
            }
            
            // Check credential validity if required
            if (options.RequireValidCredential)
            {
                var credentials = await _storage.GetCredentialsAsync(identityId, cancellationToken);
                if (!credentials.Any(c => c.IsValidOffline()))
                {
                    return OfflineVerificationResult.Failure(
                        "NO_VALID_CREDENTIAL",
                        "No valid offline credential found for identity",
                        modality);
                }
            }
            
            // Perform matching against identity templates
            var bestMatch = await FindBestMatchAsync(biometricSample, modalityTemplates, cancellationToken);
            
            if (bestMatch.matchScore >= options.MinMatchScore)
            {
                var result = OfflineVerificationResult.Success(
                    identityId,
                    modality,
                    bestMatch.matchScore,
                    qualityScore,
                    wasOffline: true);
                result.DeviceId = options.DeviceId;
                
                AddPendingSync(result);
                
                _logger.LogInformation("Offline identity verification successful. Score: {Score}", bestMatch.matchScore);
                return result;
            }
            
            return OfflineVerificationResult.NoMatch(modality, bestMatch.matchScore, qualityScore);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during offline identity verification");
            return OfflineVerificationResult.Failure("INTERNAL_ERROR", ex.Message, modality);
        }
    }
    
    /// <inheritdoc />
    public async Task CacheTemplateAsync(
        Guid identityId,
        byte[] templateData,
        BiometricModality modality,
        double qualityScore,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Caching biometric template for identity: {IdentityId}, modality: {Modality}",
            identityId, modality);
            
        // Encrypt template data
        var (encryptedData, iv, keyId) = EncryptTemplate(templateData);
        
        var template = new CachedBiometricTemplate
        {
            IdentityId = identityId,
            Modality = modality,
            EncryptedTemplate = encryptedData,
            EncryptionIV = iv,
            EncryptionKeyId = keyId,
            QualityScore = qualityScore,
            EnrolledAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30) // 30-day offline validity
        };
        
        await _storage.StoreBiometricTemplateAsync(template, cancellationToken);
        _logger.LogInformation("Template cached successfully. Template ID: {TemplateId}", template.Id);
    }
    
    /// <inheritdoc />
    public async Task RemoveCachedTemplatesAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        var templates = await _storage.GetBiometricTemplatesAsync(identityId, cancellationToken);
        foreach (var template in templates)
        {
            await _storage.RemoveBiometricTemplateAsync(template.Id, cancellationToken);
        }
        _logger.LogInformation("Removed {Count} cached templates for identity: {IdentityId}",
            templates.Count, identityId);
    }
    
    /// <inheritdoc />
    public async Task<bool> IsAvailableAsync(BiometricModality modality, CancellationToken cancellationToken = default)
    {
        var templates = await _storage.GetBiometricTemplatesByModalityAsync(modality, cancellationToken);
        return templates.Any();
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<OfflineVerificationResult>> GetPendingSyncResultsAsync(CancellationToken cancellationToken = default)
    {
        lock (_syncLock)
        {
            var pending = _pendingSync.Where(r => r.SyncPending).ToList();
            return Task.FromResult<IReadOnlyList<OfflineVerificationResult>>(pending);
        }
    }
    
    /// <inheritdoc />
    public Task MarkSyncedAsync(IEnumerable<string> transactionIds, CancellationToken cancellationToken = default)
    {
        lock (_syncLock)
        {
            var idSet = new HashSet<string>(transactionIds);
            foreach (var result in _pendingSync.Where(r => idSet.Contains(r.TransactionId)))
            {
                result.SyncPending = false;
            }
            _pendingSync.RemoveAll(r => !r.SyncPending);
        }
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Calculates the quality score of a biometric sample.
    /// In production, use actual biometric quality assessment algorithms.
    /// </summary>
    private static double CalculateQualityScore(byte[] sample, BiometricModality modality)
    {
        // Simplified quality calculation
        // In production, use NIST NFIQ for fingerprints, ISO/IEC 29794 for face, etc.
        if (sample == null || sample.Length == 0)
            return 0.0;
            
        // Basic heuristics based on sample size and entropy
        var entropy = CalculateEntropy(sample);
        var sizeFactor = Math.Min(1.0, sample.Length / 10000.0);
        
        return (entropy * 0.7 + sizeFactor * 0.3);
    }
    
    /// <summary>
    /// Calculates Shannon entropy of data.
    /// </summary>
    private static double CalculateEntropy(byte[] data)
    {
        var frequency = new int[256];
        foreach (var b in data)
            frequency[b]++;
            
        var entropy = 0.0;
        var length = (double)data.Length;
        
        foreach (var f in frequency)
        {
            if (f > 0)
            {
                var p = f / length;
                entropy -= p * Math.Log2(p);
            }
        }
        
        return entropy / 8.0; // Normalize to 0-1 range
    }
    
    /// <summary>
    /// Finds the best matching template for a biometric sample.
    /// </summary>
    private async Task<(CachedBiometricTemplate template, double matchScore)> FindBestMatchAsync(
        byte[] sample,
        IEnumerable<CachedBiometricTemplate> templates,
        CancellationToken cancellationToken)
    {
        CachedBiometricTemplate? bestTemplate = null;
        var bestScore = 0.0;
        
        foreach (var template in templates)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
                
            // Decrypt template
            var decryptedTemplate = DecryptTemplate(template);
            
            // Calculate match score
            var score = CalculateMatchScore(sample, decryptedTemplate);
            
            if (score > bestScore)
            {
                bestScore = score;
                bestTemplate = template;
            }
        }
        
        return (bestTemplate!, bestScore);
    }
    
    /// <summary>
    /// Calculates match score between a sample and template.
    /// In production, use actual biometric matching algorithms (ISO 19795).
    /// </summary>
    private static double CalculateMatchScore(byte[] sample, byte[] template)
    {
        // Simplified matching - in production use real biometric SDK
        // This is a placeholder that compares byte patterns
        
        if (sample.Length == 0 || template.Length == 0)
            return 0.0;
            
        // Use a simple correlation measure
        var minLength = Math.Min(sample.Length, template.Length);
        var matches = 0;
        
        for (var i = 0; i < minLength; i++)
        {
            if (sample[i] == template[i])
                matches++;
        }
        
        return (double)matches / minLength;
    }
    
    /// <summary>
    /// Encrypts a biometric template for secure storage.
    /// </summary>
    private static (byte[] encrypted, byte[] iv, string keyId) EncryptTemplate(byte[] template)
    {
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        aes.GenerateIV();
        
        using var encryptor = aes.CreateEncryptor();
        var encrypted = encryptor.TransformFinalBlock(template, 0, template.Length);
        
        return (encrypted, aes.IV, "key-1");
    }
    
    /// <summary>
    /// Decrypts a cached biometric template.
    /// </summary>
    private static byte[] DecryptTemplate(CachedBiometricTemplate template)
    {
        // In production, retrieve the actual key based on keyId
        // For now, return the encrypted data as-is (simplified)
        // Real implementation would use HSM-backed key management
        return template.EncryptedTemplate;
    }
    
    /// <summary>
    /// Adds a verification result to the pending sync queue.
    /// </summary>
    private void AddPendingSync(OfflineVerificationResult result)
    {
        lock (_syncLock)
        {
            _pendingSync.Add(result);
            
            // Keep queue bounded
            while (_pendingSync.Count > 10000)
            {
                _pendingSync.RemoveAt(0);
            }
        }
    }
}
