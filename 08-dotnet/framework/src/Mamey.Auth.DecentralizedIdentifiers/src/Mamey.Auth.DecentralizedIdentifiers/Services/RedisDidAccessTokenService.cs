using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using Mamey.Auth.DecentralizedIdentifiers.Handlers;

namespace Mamey.Auth.DecentralizedIdentifiers.Services;

/// <summary>
/// Redis implementation of DID access token service
/// </summary>
public class RedisDidAccessTokenService : IAccessTokenService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisDidAccessTokenService> _logger;
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromHours(1);

    public RedisDidAccessTokenService(
        IDistributedCache cache,
        ILogger<RedisDidAccessTokenService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task StoreTokenAsync(DidToken token)
    {
        try
        {
            var tokenId = GenerateTokenId(token);
            var tokenJson = JsonSerializer.Serialize(token);
            
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(token.Expires - DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await _cache.SetStringAsync(tokenId, tokenJson, options);
            
            _logger.LogDebug("Stored DID token in Redis: {TokenId}", tokenId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store DID token in Redis");
            throw;
        }
    }

    public async Task<DidToken> GetTokenAsync(string tokenId)
    {
        try
        {
            var tokenJson = await _cache.GetStringAsync(tokenId);
            if (string.IsNullOrEmpty(tokenJson))
            {
                return null;
            }

            var token = JsonSerializer.Deserialize<DidToken>(tokenJson);
            return token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get DID token from Redis: {TokenId}", tokenId);
            throw;
        }
    }

    public async Task RevokeTokenAsync(string tokenId)
    {
        try
        {
            await _cache.RemoveAsync(tokenId);
            
            // Store in revoked tokens set
            var revokedKey = $"revoked:{tokenId}";
            await _cache.SetStringAsync(revokedKey, DateTime.UtcNow.ToString("O"), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _defaultExpiration
            });
            
            _logger.LogDebug("Revoked DID token in Redis: {TokenId}", tokenId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to revoke DID token in Redis: {TokenId}", tokenId);
            throw;
        }
    }

    public async Task<bool> IsTokenValidAsync(string tokenId)
    {
        try
        {
            // Check if token is revoked
            var revokedKey = $"revoked:{tokenId}";
            var revokedAt = await _cache.GetStringAsync(revokedKey);
            if (!string.IsNullOrEmpty(revokedAt))
            {
                return false;
            }

            // Check if token exists
            var token = await GetTokenAsync(tokenId);
            if (token == null)
            {
                return false;
            }

            // Check if token is expired
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var isValid = token.Expires > now;

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check token validity in Redis: {TokenId}", tokenId);
            return false;
        }
    }

    private static string GenerateTokenId(DidToken token)
    {
        return $"{token.Id}:{token.Expires}";
    }
}





