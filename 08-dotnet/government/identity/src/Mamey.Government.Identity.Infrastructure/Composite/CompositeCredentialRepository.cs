using System;
using System.Collections.Generic;
using System.Linq;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Government.Identity.Domain.Repositories;
using Mamey.Government.Identity.Infrastructure.EF.Repositories;
using Mamey.Government.Identity.Infrastructure.Mongo.Repositories;
using Mamey.Government.Identity.Infrastructure.Redis.Repositories;
using Mamey.Microservice.Infrastructure.Mongo.Documents;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Identity.Infrastructure.Composite;

internal class CompositeCredentialRepository : ICredentialRepository
{
    private readonly CredentialMongoRepository _mongoRepo;
    private readonly CredentialRedisRepository _redisRepo;
    private readonly CredentialPostgresRepository _postgresRepo;
    private readonly ILogger<CompositeCredentialRepository> _logger;

    public CompositeCredentialRepository(
        CredentialMongoRepository mongoRepo,
        CredentialRedisRepository redisRepo,
        CredentialPostgresRepository postgresRepo,
        ILogger<CompositeCredentialRepository> logger)
    {
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _postgresRepo = postgresRepo;
        _logger = logger;
    }

    public async Task AddAsync(Credential credential, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(credential, cancellationToken);
        Try(() => _redisRepo.AddAsync(credential, cancellationToken));
        Try(() => _mongoRepo.AddAsync(credential, cancellationToken));
    }

    public async Task UpdateAsync(Credential credential, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(credential, cancellationToken);
        Try(() => _redisRepo.UpdateAsync(credential, cancellationToken));
        Try(() => _mongoRepo.UpdateAsync(credential, cancellationToken));
    }

    public async Task DeleteAsync(CredentialId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        Try(() => _redisRepo.DeleteAsync(id, cancellationToken));
        Try(() => _mongoRepo.DeleteAsync(id, cancellationToken));
    }

    public Task<IReadOnlyList<Credential>> BrowseAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(cancellationToken);

    public async Task<Credential> GetAsync(CredentialId id, CancellationToken cancellationToken = default)
    {
        // Try Redis → Mongo → Postgres
        try
        {
            var cred = await _redisRepo.GetAsync(id, cancellationToken);
            if (cred != null) return cred;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetAsync(id, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }
        
        return await _postgresRepo.GetAsync(id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(CredentialId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis exists failed, trying Mongo"); }
        
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo exists failed, trying Postgres"); }
        
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<Credential>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var cached = await _redisRepo.GetByUserIdAsync(userId, cancellationToken);
            if (cached.Any()) return cached;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetByUserIdAsync(userId, cancellationToken);
            if (fromMongo.Any()) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }
        
        return await _postgresRepo.GetByUserIdAsync(userId, cancellationToken);
    }

    public Task<IReadOnlyList<Credential>> GetByTypeAsync(CredentialType type, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByTypeAsync(type, cancellationToken);

    public Task<IReadOnlyList<Credential>> GetByStatusAsync(CredentialStatus status, CancellationToken cancellationToken = default)
        => _postgresRepo.GetByStatusAsync(status, cancellationToken);

    public async Task<IReadOnlyList<Credential>> GetByUserIdAndTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default)
    {
        try
        {
            var cached = await _redisRepo.GetByUserIdAndTypeAsync(userId, type, cancellationToken);
            if (cached.Any()) return cached;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetByUserIdAndTypeAsync(userId, type, cancellationToken);
            if (fromMongo.Any()) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo read failed, trying Postgres"); }
        
        return await _postgresRepo.GetByUserIdAndTypeAsync(userId, type, cancellationToken);
    }

    public Task<IReadOnlyList<Credential>> GetActiveCredentialsAsync(UserId userId, CancellationToken cancellationToken = default)
        => _postgresRepo.GetActiveCredentialsAsync(userId, cancellationToken);

    public Task<IReadOnlyList<Credential>> GetExpiredCredentialsAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.GetExpiredCredentialsAsync(cancellationToken);

    public Task<IReadOnlyList<Credential>> GetCredentialsExpiringSoonAsync(DateTime expirationThreshold, CancellationToken cancellationToken = default)
        => _postgresRepo.GetCredentialsExpiringSoonAsync(expirationThreshold, cancellationToken);

    public async Task<bool> HasActiveCredentialOfTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default)
    {
        try
        {
            if (await _redisRepo.HasActiveCredentialOfTypeAsync(userId, type, cancellationToken)) return true;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis read failed, trying Postgres"); }
        
        return await _postgresRepo.HasActiveCredentialOfTypeAsync(userId, type, cancellationToken);
    }

    public Task<int> GetActiveCredentialCountAsync(UserId userId, CancellationToken cancellationToken = default)
        => _postgresRepo.GetActiveCredentialCountAsync(userId, cancellationToken);

    public Task DeleteExpiredCredentialsAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.DeleteExpiredCredentialsAsync(cancellationToken);

    public Task DeleteCredentialsByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
        => _postgresRepo.DeleteCredentialsByUserIdAsync(userId, cancellationToken);

    public Task DeleteCredentialsByUserIdAndTypeAsync(UserId userId, CredentialType type, CancellationToken cancellationToken = default)
        => _postgresRepo.DeleteCredentialsByUserIdAndTypeAsync(userId, type, cancellationToken);

    private void Try(Func<Task> action)
    {
        try { action().GetAwaiter().GetResult(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Best-effort propagation failed"); }
    }
}


