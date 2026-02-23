using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Persistence.Redis;
using Mamey.Types;

namespace Mamey.Government.Identity.Infrastructure.Redis.Repositories;

internal class CredentialRedisRepository : ICredentialRepository
{
    private readonly ICache _cache;
    private const string CredentialPrefix = "credential:";
    private const string UserPrefix = "credential:user:";
    private const string UserTypePrefix = "credential:user-type:";

    public CredentialRedisRepository(ICache cache)
    {
        _cache = cache;
    }

    public async Task AddAsync(Credential credential, CancellationToken cancellationToken = default)
    {
        // Cache with TTL based on expiration time or default 24 hours
        var ttl = credential.ExpiresAt.HasValue 
            ? credential.ExpiresAt.Value - DateTime.UtcNow 
            : TimeSpan.FromHours(24);
        
        if (ttl <= TimeSpan.Zero) return;

        await _cache.SetAsync($"{CredentialPrefix}{credential.Id.Value}", credential, ttl);
        
        // Index by UserId
        await _cache.AddToSetAsync($"{UserPrefix}{credential.UserId.Value}", credential.Id.Value);
        
        // Index by UserId + Type
        await _cache.SetAsync($"{UserTypePrefix}{credential.UserId.Value}:{credential.Type}", credential.Id.Value, ttl);
    }

    public async Task UpdateAsync(Credential credential, CancellationToken cancellationToken = default)
    {
        await AddAsync(credential, cancellationToken);
    }

    public async Task DeleteAsync(CredentialId id, CancellationToken cancellationToken = default)
    {
        var credential = await GetAsync(id, cancellationToken);
        if (credential == null) return;

        await _cache.DeleteAsync<Credential>($"{CredentialPrefix}{id.Value}");
        await _cache.DeleteFromSetAsync<Guid>($"{UserPrefix}{credential.UserId.Value}", id.Value);
        await _cache.DeleteAsync<Guid>($"{UserTypePrefix}{credential.UserId.Value}:{credential.Type}");
    }

    public async Task<IReadOnlyList<Credential>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<Credential>();
    }

    public async Task<Credential> GetAsync(CredentialId id, CancellationToken cancellationToken = default)
        => await _cache.GetAsync<Credential>($"{CredentialPrefix}{id.Value}");

    public async Task<bool> ExistsAsync(CredentialId id, CancellationToken cancellationToken = default)
        => await _cache.KeyExistsAsync($"{CredentialPrefix}{id.Value}");

    public async Task<IReadOnlyList<Credential>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var credentialIds = await _cache.GetManyAsync<Guid>($"{UserPrefix}{userId.Value}");
        var credentials = new List<Credential>();
        
        foreach (var credentialId in credentialIds)
        {
            var credential = await GetAsync(new CredentialId(credentialId), cancellationToken);
            if (credential != null)
            {
                credentials.Add(credential);
            }
        }
        
        return credentials;
    }

    public async Task<IReadOnlyList<Credential>> GetByTypeAsync(CredentialType type, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<Credential>();
    }

    public async Task<IReadOnlyList<Credential>> GetByStatusAsync(CredentialStatus status, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<Credential>();
    }

    public async Task<IReadOnlyList<Credential>> GetByUserIdAndTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default)
    {
        var credentialId = await _cache.GetAsync<Guid>($"{UserTypePrefix}{userId.Value}:{type}");
        if (credentialId == Guid.Empty) return new List<Credential>();
        
        var credential = await GetAsync(new CredentialId(credentialId), cancellationToken);
        return credential != null ? new List<Credential> { credential } : new List<Credential>();
    }

    public async Task<IReadOnlyList<Credential>> GetActiveCredentialsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var credentials = await GetByUserIdAsync(userId, cancellationToken);
        var now = DateTime.UtcNow;
        return credentials.Where(c => c.Status == CredentialStatus.Active && 
                                     (!c.ExpiresAt.HasValue || c.ExpiresAt.Value > now)).ToList();
    }

    public async Task<IReadOnlyList<Credential>> GetExpiredCredentialsAsync(CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<Credential>();
    }

    public async Task<IReadOnlyList<Credential>> GetCredentialsExpiringSoonAsync(DateTime expirationThreshold, CancellationToken cancellationToken = default)
    {
        // Not efficient for Redis
        return new List<Credential>();
    }

    public async Task<bool> HasActiveCredentialOfTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default)
    {
        var credential = await GetByUserIdAndTypeAsync(userId, type, cancellationToken);
        if (!credential.Any()) return false;
        
        var cred = credential.First();
        var now = DateTime.UtcNow;
        return cred.Status == CredentialStatus.Active && (!cred.ExpiresAt.HasValue || cred.ExpiresAt.Value > now);
    }

    public async Task<int> GetActiveCredentialCountAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var credentials = await GetActiveCredentialsAsync(userId, cancellationToken);
        return credentials.Count;
    }

    public async Task DeleteExpiredCredentialsAsync(CancellationToken cancellationToken = default)
    {
        // Expired items are automatically removed by Redis TTL
    }

    public async Task DeleteCredentialsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var credentials = await GetByUserIdAsync(userId, cancellationToken);
        foreach (var credential in credentials)
        {
            await DeleteAsync(credential.Id, cancellationToken);
        }
    }

    public async Task DeleteCredentialsByUserIdAndTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default)
    {
        var credential = await GetByUserIdAndTypeAsync(userId, type, cancellationToken);
        if (credential.Any())
        {
            await DeleteAsync(credential.First().Id, cancellationToken);
        }
    }
}

