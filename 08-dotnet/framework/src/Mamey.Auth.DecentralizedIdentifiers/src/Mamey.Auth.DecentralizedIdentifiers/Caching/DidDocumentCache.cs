using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Persistence.Redis;

namespace Mamey.Auth.DecentralizedIdentifiers.Caching;

/// <summary>
/// In-memory implementation of DID document cache
/// </summary>
public class DidDocumentCache : IDidDocumentCache
{
    private readonly ICache _cache;
    private readonly ILogger<DidDocumentCache> _logger;

    public DidDocumentCache(ILogger<DidDocumentCache> logger, ICache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public async Task<DidDocument?> GetAsync(string did)
    {
        try
        {
            var cachedDoc = await  _cache.GetAsync<CachedDocument>(did);
            if (cachedDoc is null)
            {
                return null;
            }

            // Check if expired
            if (cachedDoc.ExpiresAt < DateTime.UtcNow)
            {
                await _cache.DeleteAsync<CachedDocument>(did);
                return null;
            }

            return cachedDoc.Document;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get DID document from cache: {Did}", did);
            return null;
        }
    }

    public Task<Dictionary<string, DidDocument>> GetManyAsync(params string[] dids)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync(string did, DidDocument document, int ttlMinutes = 60)
    {
        try
        {
            var cachedDoc = new CachedDocument
            {
                Document = document,
                ExpiresAt = DateTime.UtcNow.AddMinutes(ttlMinutes)
            };

            _cache.SetAsync(did, cachedDoc, TimeSpan.FromMinutes(ttlMinutes));
            
            _logger.LogDebug("Cached DID document: {Did}, TTL: {TtlMinutes} minutes", did, ttlMinutes);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cache DID document: {Did}", did);
            throw;
        }
    }

    public Task SetManyAsync(Dictionary<string, DidDocument> documents, int ttlMinutes = 60)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveAsync(string did)
    {
        try
        {
            await _cache.DeleteAsync<CachedDocument>(did);
            _logger.LogDebug("Removed DID document from cache: {Did}", did);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove DID document from cache: {Did}", did);
            throw;
        }
    }

    public Task RemoveManyAsync(params string[] dids)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsAsync(string did)
    {
        try
        {
            var cachedDoc = await _cache.GetAsync<CachedDocument>(did);
            if (cachedDoc is null)
            {
                return false;
            }

            // Check if expired
            if (cachedDoc.ExpiresAt < DateTime.UtcNow)
            {
                await _cache.DeleteAsync<CachedDocument>(did);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check DID document existence in cache: {Did}", did);
            return false;
        }
    }

    public Task<CacheStatistics> GetStatisticsAsync()
    {
        throw new NotImplementedException();
    }

    public Task ClearAsync()
    {
        throw new NotImplementedException();
    }

    public Task InvalidateAsync(string did)
    {
        throw new NotImplementedException();
    }

    private class CachedDocument
    {
        public DidDocument Document { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}





