using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Infrastructure.EF.Repositories;
using Mamey.FWID.Identities.Infrastructure.Mongo.Repositories;
using Mamey.FWID.Identities.Infrastructure.Redis.Repositories;
using Mamey.Types;
using Microsoft.Extensions.Logging;
using DomainEntities = Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Infrastructure.Composite;

internal class CompositeIdentityRepository : IIdentityRepository
{
    private readonly IdentityMongoRepository _mongoRepo;
    private readonly IdentityRedisRepository _redisRepo;
    private readonly IdentityPostgresRepository _postgresRepo;
    private readonly ILogger<CompositeIdentityRepository> _logger;

    public CompositeIdentityRepository(
        IdentityMongoRepository mongoRepo,
        IdentityRedisRepository redisRepo,
        IdentityPostgresRepository postgresRepo,
        ILogger<CompositeIdentityRepository> logger)
    {
        _mongoRepo = mongoRepo;
        _redisRepo = redisRepo;
        _postgresRepo = postgresRepo;
        _logger = logger;
    }

    public async Task AddAsync(Identity entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.AddAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.AddAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task UpdateAsync(Identity entity, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateAsync(entity, cancellationToken);
        await TryAsync(() => _redisRepo.UpdateAsync(entity, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.UpdateAsync(entity, cancellationToken), "MongoDB");
    }

    public async Task DeleteAsync(DomainEntities.IdentityId id, CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteAsync(id, cancellationToken);
        await TryAsync(() => _redisRepo.DeleteAsync(id, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.DeleteAsync(id, cancellationToken), "MongoDB");
    }

    public async Task<Identity?> GetAsync(DomainEntities.IdentityId id, CancellationToken cancellationToken = default)
    {
        // Try Redis → Mongo → Postgres
        try
        {
            var fromRedis = await _redisRepo.GetAsync(id, cancellationToken);
            if (fromRedis != null) return fromRedis;
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

    public Task<IReadOnlyList<Identity>> BrowseAsync(CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(cancellationToken);

    public async Task<bool> ExistsAsync(DomainEntities.IdentityId id, CancellationToken cancellationToken = default)
    {
        try { if (await _redisRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis exists failed, trying Mongo"); }
        
        try { if (await _mongoRepo.ExistsAsync(id, cancellationToken)) return true; }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo exists failed, trying Postgres"); }
        
        return await _postgresRepo.ExistsAsync(id, cancellationToken);
    }

    #region Pagination

    public Task<PagedResult<Identity>> BrowseAsync(IPagedQuery query, CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(query, cancellationToken);

    public Task<PagedResult<Identity>> BrowseAsync(
        IPagedQuery query,
        Expression<Func<Identity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => _postgresRepo.BrowseAsync(query, predicate, cancellationToken);

    #endregion

    #region Expression-Based Queries

    public Task<IReadOnlyList<Identity>> FindAsync(
        Expression<Func<Identity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => _postgresRepo.FindAsync(predicate, cancellationToken);

    public Task<Identity?> GetSingleOrDefaultAsync(
        Expression<Func<Identity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => _postgresRepo.GetSingleOrDefaultAsync(predicate, cancellationToken);

    public Task<int> CountAsync(
        Expression<Func<Identity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
        => _postgresRepo.CountAsync(predicate, cancellationToken);

    public Task<bool> ExistsAsync(
        Expression<Func<Identity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => _postgresRepo.ExistsAsync(predicate, cancellationToken);

    #endregion

    #region Bulk Operations

    public async Task AddRangeAsync(
        IEnumerable<Identity> entities,
        CancellationToken cancellationToken = default)
    {
        await _postgresRepo.AddRangeAsync(entities, cancellationToken);
        await TryAsync(() => _redisRepo.AddRangeAsync(entities, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.AddRangeAsync(entities, cancellationToken), "MongoDB");
    }

    public async Task UpdateRangeAsync(
        IEnumerable<Identity> entities,
        CancellationToken cancellationToken = default)
    {
        await _postgresRepo.UpdateRangeAsync(entities, cancellationToken);
        await TryAsync(() => _redisRepo.UpdateRangeAsync(entities, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.UpdateRangeAsync(entities, cancellationToken), "MongoDB");
    }

    public async Task DeleteRangeAsync(
        IEnumerable<DomainEntities.IdentityId> ids,
        CancellationToken cancellationToken = default)
    {
        await _postgresRepo.DeleteRangeAsync(ids, cancellationToken);
        await TryAsync(() => _redisRepo.DeleteRangeAsync(ids, cancellationToken), "Redis");
        await TryAsync(() => _mongoRepo.DeleteRangeAsync(ids, cancellationToken), "MongoDB");
    }

    #endregion

    #region Pessimistic Locking

    /// <summary>
    /// Gets an identity with an exclusive lock (SELECT FOR UPDATE).
    /// Pessimistic locking only applies to PostgreSQL (source of truth).
    /// </summary>
    public Task<Identity?> GetWithLockAsync(DomainEntities.IdentityId id, CancellationToken cancellationToken = default)
        => _postgresRepo.GetWithLockAsync(id, cancellationToken);

    /// <summary>
    /// Gets an identity matching the predicate with an exclusive lock (SELECT FOR UPDATE).
    /// Pessimistic locking only applies to PostgreSQL (source of truth).
    /// </summary>
    public Task<Identity?> GetSingleOrDefaultWithLockAsync(
        Expression<Func<Identity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => _postgresRepo.GetSingleOrDefaultWithLockAsync(predicate, cancellationToken);

    #endregion

    #region Lookup By External Identifiers

    public async Task<Identity?> GetByDidAsync(string did, CancellationToken cancellationToken = default)
    {
        // Try Redis → Mongo → Postgres
        try
        {
            var fromRedis = await _redisRepo.GetByDidAsync(did, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis GetByDid failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetByDidAsync(did, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo GetByDid failed, trying Postgres"); }
        
        return await _postgresRepo.GetByDidAsync(did, cancellationToken);
    }

    public async Task<Identity?> GetByAzureUserIdAsync(string azureUserId, CancellationToken cancellationToken = default)
    {
        // Try Redis → Mongo → Postgres
        try
        {
            var fromRedis = await _redisRepo.GetByAzureUserIdAsync(azureUserId, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis GetByAzureUserId failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetByAzureUserIdAsync(azureUserId, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo GetByAzureUserId failed, trying Postgres"); }
        
        return await _postgresRepo.GetByAzureUserIdAsync(azureUserId, cancellationToken);
    }

    public async Task<Identity?> GetByServiceIdAsync(string serviceId, CancellationToken cancellationToken = default)
    {
        // Try Redis → Mongo → Postgres
        try
        {
            var fromRedis = await _redisRepo.GetByServiceIdAsync(serviceId, cancellationToken);
            if (fromRedis != null) return fromRedis;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Redis GetByServiceId failed, trying Mongo"); }
        
        try
        {
            var fromMongo = await _mongoRepo.GetByServiceIdAsync(serviceId, cancellationToken);
            if (fromMongo != null) return fromMongo;
        }
        catch (Exception ex) { _logger.LogWarning(ex, "Mongo GetByServiceId failed, trying Postgres"); }
        
        return await _postgresRepo.GetByServiceIdAsync(serviceId, cancellationToken);
    }

    #endregion

    private async Task TryAsync(Func<Task> action, string storeName)
    {
        try 
        { 
            await action(); 
            _logger.LogDebug("Successfully propagated to {StoreName}", storeName);
        }
        catch (Exception ex) 
        { 
            _logger.LogWarning(ex, "Best-effort propagation to {StoreName} failed: {ErrorMessage}", storeName, ex.Message); 
        }
    }
}

