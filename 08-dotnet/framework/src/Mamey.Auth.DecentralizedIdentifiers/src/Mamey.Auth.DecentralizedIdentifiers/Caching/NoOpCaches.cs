using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mamey.Auth.DecentralizedIdentifiers.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.Caching;

/// <summary>
/// No-op implementation of DID document cache when caching is disabled
/// </summary>
public class NoOpDidDocumentCache : IDidDocumentCache
{
    private readonly ILogger<NoOpDidDocumentCache> _logger;

    public NoOpDidDocumentCache(ILogger<NoOpDidDocumentCache> logger = null)
    {
        _logger = logger;
    }

    public Task<DidDocument> GetAsync(string did)
    {
        _logger?.LogDebug("DID document cache disabled - returning null for {Did}", did);
        return Task.FromResult<DidDocument>(null);
    }

    public Task<Dictionary<string, DidDocument>> GetManyAsync(params string[] dids)
    {
        _logger?.LogDebug("DID document cache disabled - returning empty dictionary for {Count} DIDs", dids?.Length ?? 0);
        return Task.FromResult(new Dictionary<string, DidDocument>());
    }

    public Task SetAsync(string did, DidDocument document, int ttlMinutes = 60)
    {
        _logger?.LogDebug("DID document cache disabled - ignoring set for {Did}", did);
        return Task.CompletedTask;
    }

    public Task SetManyAsync(Dictionary<string, DidDocument> documents, int ttlMinutes = 60)
    {
        _logger?.LogDebug("DID document cache disabled - ignoring set for {Count} documents", documents?.Count ?? 0);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string did)
    {
        _logger?.LogDebug("DID document cache disabled - ignoring remove for {Did}", did);
        return Task.CompletedTask;
    }

    public Task RemoveManyAsync(params string[] dids)
    {
        _logger?.LogDebug("DID document cache disabled - ignoring remove for {Count} DIDs", dids?.Length ?? 0);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string did)
    {
        _logger?.LogDebug("DID document cache disabled - returning false for {Did}", did);
        return Task.FromResult(false);
    }

    public Task<CacheStatistics> GetStatisticsAsync()
    {
        return Task.FromResult(new CacheStatistics
        {
            TotalEntries = 0,
            HitCount = 0,
            MissCount = 0,
            LastAccessed = DateTime.UtcNow
        });
    }

    public Task ClearAsync()
    {
        _logger?.LogDebug("DID document cache disabled - ignoring clear");
        return Task.CompletedTask;
    }

    public Task InvalidateAsync(string did)
    {
        _logger?.LogDebug("DID document cache disabled - ignoring invalidate for {Did}", did);
        return Task.CompletedTask;
    }
}

/// <summary>
/// No-op implementation of verification result cache when caching is disabled
/// </summary>
public class NoOpVerificationResultCache : IVerificationResultCache
{
    private readonly ILogger<NoOpVerificationResultCache> _logger;

    public NoOpVerificationResultCache(ILogger<NoOpVerificationResultCache> logger = null)
    {
        _logger = logger;
    }

    public Task<VerificationResult> GetAsync(string key)
    {
        _logger?.LogDebug("Verification result cache disabled - returning null for {Key}", key);
        return Task.FromResult<VerificationResult>(null);
    }

    public Task SetAsync(string key, VerificationResult result, int ttlMinutes = 30)
    {
        _logger?.LogDebug("Verification result cache disabled - ignoring set for {Key}", key);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _logger?.LogDebug("Verification result cache disabled - ignoring remove for {Key}", key);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        _logger?.LogDebug("Verification result cache disabled - returning false for {Key}", key);
        return Task.FromResult(false);
    }

    public Task ClearAsync()
    {
        _logger?.LogDebug("Verification result cache disabled - ignoring clear");
        return Task.CompletedTask;
    }
}

/// <summary>
/// No-op implementation of credential status cache when caching is disabled
/// </summary>
public class NoOpCredentialStatusCache : ICredentialStatusCache
{
    private readonly ILogger<NoOpCredentialStatusCache> _logger;

    public NoOpCredentialStatusCache(ILogger<NoOpCredentialStatusCache> logger = null)
    {
        _logger = logger;
    }

    public Task<CredentialStatusResult> GetAsync(string credentialId)
    {
        _logger?.LogDebug("Credential status cache disabled - returning null for {CredentialId}", credentialId);
        return Task.FromResult<CredentialStatusResult>(null);
    }

    public Task SetAsync(string credentialId, CredentialStatusResult status, int ttlMinutes = 15)
    {
        _logger?.LogDebug("Credential status cache disabled - ignoring set for {CredentialId}", credentialId);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string credentialId)
    {
        _logger?.LogDebug("Credential status cache disabled - ignoring remove for {CredentialId}", credentialId);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string credentialId)
    {
        _logger?.LogDebug("Credential status cache disabled - returning false for {CredentialId}", credentialId);
        return Task.FromResult(false);
    }

    public Task ClearAsync()
    {
        _logger?.LogDebug("Credential status cache disabled - ignoring clear");
        return Task.CompletedTask;
    }
}

