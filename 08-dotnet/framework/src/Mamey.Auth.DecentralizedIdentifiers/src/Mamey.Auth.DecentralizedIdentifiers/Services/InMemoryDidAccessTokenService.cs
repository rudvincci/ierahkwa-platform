using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mamey.Auth.DecentralizedIdentifiers.Handlers;
using Mamey.Auth.DecentralizedIdentifiers.Services;

namespace Mamey.Auth.DecentralizedIdentifiers.Services;

/// <summary>
/// In-memory implementation of DID access token service
/// </summary>
public class InMemoryDidAccessTokenService : IAccessTokenService
{
    private readonly ConcurrentDictionary<string, DidToken> _tokens = new();
    private readonly ConcurrentDictionary<string, DateTime> _revokedTokens = new();
    private readonly ILogger<InMemoryDidAccessTokenService> _logger;

    public InMemoryDidAccessTokenService(ILogger<InMemoryDidAccessTokenService> logger)
    {
        _logger = logger;
    }

    public Task StoreTokenAsync(DidToken token)
    {
        try
        {
            var tokenId = GenerateTokenId(token);
            _tokens.TryAdd(tokenId, token);
            
            _logger.LogDebug("Stored DID token: {TokenId}", tokenId);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store DID token");
            throw;
        }
    }

    public Task<DidToken> GetTokenAsync(string tokenId)
    {
        try
        {
            if (_revokedTokens.ContainsKey(tokenId))
            {
                return Task.FromResult<DidToken>(null);
            }

            _tokens.TryGetValue(tokenId, out var token);
            return Task.FromResult(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get DID token: {TokenId}", tokenId);
            throw;
        }
    }

    public Task RevokeTokenAsync(string tokenId)
    {
        try
        {
            _revokedTokens.TryAdd(tokenId, DateTime.UtcNow);
            _tokens.TryRemove(tokenId, out _);
            
            _logger.LogDebug("Revoked DID token: {TokenId}", tokenId);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to revoke DID token: {TokenId}", tokenId);
            throw;
        }
    }

    public Task<bool> IsTokenValidAsync(string tokenId)
    {
        try
        {
            if (_revokedTokens.ContainsKey(tokenId))
            {
                return Task.FromResult(false);
            }

            if (!_tokens.TryGetValue(tokenId, out var token))
            {
                return Task.FromResult(false);
            }

            // Check if token is expired
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var isValid = token.Expires > now;

            return Task.FromResult(isValid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check token validity: {TokenId}", tokenId);
            return Task.FromResult(false);
        }
    }

    private static string GenerateTokenId(DidToken token)
    {
        return $"{token.Id}:{token.Expires}";
    }
}





