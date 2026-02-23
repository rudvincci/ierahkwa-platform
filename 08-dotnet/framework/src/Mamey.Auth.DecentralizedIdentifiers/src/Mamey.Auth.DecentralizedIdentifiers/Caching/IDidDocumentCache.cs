using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.Caching;

/// <summary>
/// Enhanced interface for DID document caching with Redis support
/// </summary>
public interface IDidDocumentCache
{
    /// <summary>
    /// Get DID document from cache
    /// </summary>
    /// <param name="did">DID to get</param>
    /// <returns>Cached DID document</returns>
    Task<DidDocument?> GetAsync(string did);
    
    /// <summary>
    /// Get multiple DID documents from cache
    /// </summary>
    /// <param name="dids">DIDs to get</param>
    /// <returns>Dictionary of cached DID documents</returns>
    Task<Dictionary<string, DidDocument>> GetManyAsync(params string[] dids);
    
    /// <summary>
    /// Set DID document in cache
    /// </summary>
    /// <param name="did">DID</param>
    /// <param name="document">DID document</param>
    /// <param name="ttlMinutes">Time to live in minutes</param>
    /// <returns>Task</returns>
    Task SetAsync(string did, DidDocument document, int ttlMinutes = 60);
    
    /// <summary>
    /// Set multiple DID documents in cache
    /// </summary>
    /// <param name="documents">Dictionary of DID documents to cache</param>
    /// <param name="ttlMinutes">Time to live in minutes</param>
    /// <returns>Task</returns>
    Task SetManyAsync(Dictionary<string, DidDocument> documents, int ttlMinutes = 60);
    
    /// <summary>
    /// Remove DID document from cache
    /// </summary>
    /// <param name="did">DID to remove</param>
    /// <returns>Task</returns>
    Task RemoveAsync(string did);
    
    /// <summary>
    /// Remove multiple DID documents from cache
    /// </summary>
    /// <param name="dids">DIDs to remove</param>
    /// <returns>Task</returns>
    Task RemoveManyAsync(params string[] dids);
    
    /// <summary>
    /// Check if DID document is cached
    /// </summary>
    /// <param name="did">DID to check</param>
    /// <returns>True if cached</returns>
    Task<bool> ExistsAsync(string did);
    
    /// <summary>
    /// Get cache statistics
    /// </summary>
    /// <returns>Cache statistics</returns>
    Task<CacheStatistics> GetStatisticsAsync();
    
    /// <summary>
    /// Clear all cached DID documents
    /// </summary>
    /// <returns>Task</returns>
    Task ClearAsync();
    
    /// <summary>
    /// Invalidate cache for a specific DID
    /// </summary>
    /// <param name="did">DID to invalidate</param>
    /// <returns>Task</returns>
    Task InvalidateAsync(string did);
}

/// <summary>
/// Interface for verification result caching
/// </summary>
public interface IVerificationResultCache
{
    /// <summary>
    /// Get verification result from cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <returns>Cached verification result</returns>
    Task<VerificationResult> GetAsync(string key);
    
    /// <summary>
    /// Set verification result in cache
    /// </summary>
    /// <param name="key">Cache key</param>
    /// <param name="result">Verification result</param>
    /// <param name="ttlMinutes">Time to live in minutes</param>
    /// <returns>Task</returns>
    Task SetAsync(string key, VerificationResult result, int ttlMinutes = 30);
    
    /// <summary>
    /// Remove verification result from cache
    /// </summary>
    /// <param name="key">Cache key to remove</param>
    /// <returns>Task</returns>
    Task RemoveAsync(string key);
    
    /// <summary>
    /// Check if verification result is cached
    /// </summary>
    /// <param name="key">Cache key to check</param>
    /// <returns>True if cached</returns>
    Task<bool> ExistsAsync(string key);
    
    /// <summary>
    /// Clear all verification results
    /// </summary>
    /// <returns>Task</returns>
    Task ClearAsync();
}

/// <summary>
/// Interface for credential status caching
/// </summary>
public interface ICredentialStatusCache
{
    /// <summary>
    /// Get credential status from cache
    /// </summary>
    /// <param name="credentialId">Credential ID</param>
    /// <returns>Cached credential status</returns>
    Task<CredentialStatusResult> GetAsync(string credentialId);
    
    /// <summary>
    /// Set credential status in cache
    /// </summary>
    /// <param name="credentialId">Credential ID</param>
    /// <param name="status">Credential status</param>
    /// <param name="ttlMinutes">Time to live in minutes</param>
    /// <returns>Task</returns>
    Task SetAsync(string credentialId, CredentialStatusResult status, int ttlMinutes = 15);
    
    /// <summary>
    /// Remove credential status from cache
    /// </summary>
    /// <param name="credentialId">Credential ID to remove</param>
    /// <returns>Task</returns>
    Task RemoveAsync(string credentialId);
    
    /// <summary>
    /// Check if credential status is cached
    /// </summary>
    /// <param name="credentialId">Credential ID to check</param>
    /// <returns>True if cached</returns>
    Task<bool> ExistsAsync(string credentialId);
    
    /// <summary>
    /// Clear all credential statuses
    /// </summary>
    /// <returns>Task</returns>
    Task ClearAsync();
}

/// <summary>
/// Verification result for caching
/// </summary>
public class VerificationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;
    public string VerificationMethod { get; set; } = string.Empty;
    public TimeSpan ProcessingTime { get; set; }
}

/// <summary>
/// Cache statistics
/// </summary>
public class CacheStatistics
{
    public int TotalEntries { get; set; }
    public int HitCount { get; set; }
    public int MissCount { get; set; }
    public double HitRatio => TotalRequests > 0 ? (double)HitCount / TotalRequests : 0;
    public int TotalRequests => HitCount + MissCount;
    public DateTime LastAccessed { get; set; } = DateTime.UtcNow;
    public long MemoryUsageBytes { get; set; }
    public Dictionary<string, int> MethodStatistics { get; set; } = new();
}