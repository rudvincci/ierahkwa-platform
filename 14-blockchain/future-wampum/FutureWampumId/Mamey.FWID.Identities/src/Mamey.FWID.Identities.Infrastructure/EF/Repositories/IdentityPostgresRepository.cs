using System.Collections.Immutable;
using System.Data;
using System.Linq.Expressions;
using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using DomainEntities = Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Infrastructure.EF.Repositories;

/// <summary>
/// PostgreSQL repository implementation for Identity entities.
/// This is the source of truth (write model) for identities.
/// </summary>
internal class IdentityPostgresRepository : IIdentityRepository, IDisposable
{
    private readonly IdentityDbContext _dbContext;
    private readonly SecurityAttributeProcessor _securityProcessor;

    public IdentityPostgresRepository(IdentityDbContext dbContext, SecurityAttributeProcessor securityProcessor)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _securityProcessor = securityProcessor ?? throw new ArgumentNullException(nameof(securityProcessor));
    }

    public async Task<IReadOnlyList<Identity>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Identitys.ToListAsync(cancellationToken);
        // Process security attributes after loading (decrypt/hash verification)
        foreach (var entity in entities)
        {
            ProcessSecurityAttributesFromStorage(entity);
        }
        return ImmutableList.CreateRange(entities);
    }

    public async Task<Identity?> GetAsync(DomainEntities.IdentityId id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Identitys
            .Where(c => c.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
        
        if (entity != null)
        {
            // Process security attributes after loading (decrypt/hash verification)
            ProcessSecurityAttributesFromStorage(entity);
        }
        
        return entity;
    }

    public async Task<bool> ExistsAsync(DomainEntities.IdentityId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Identitys
            .AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task AddAsync(Identity entity, CancellationToken cancellationToken = default)
    {
        // Process security attributes before saving (encrypt/hash)
        ProcessSecurityAttributesToStorage(entity);
        
        // Version is managed by AggregateRoot<T>:
        // - AggregateRoot initializes Version to 1
        // - AddEvent() increments Version when first event is added
        // - IncrementVersion() can be called explicitly
        // Repository just persists the Version value as-is
        
        await _dbContext.Identitys.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Identity entity, CancellationToken cancellationToken = default)
    {
        // Process security attributes before saving (encrypt/hash)
        ProcessSecurityAttributesToStorage(entity);
        
        // Version is managed by AggregateRoot<T>:
        // - Domain methods (UpdateContactInformation, UpdateZone, etc.) call IncrementVersion() explicitly
        // - AddEvent() also increments Version when first event is added
        // - EF Core will throw DbUpdateConcurrencyException if Version in DB doesn't match entity.Version
        // Repository just persists the Version value as-is from the domain model
        
        _dbContext.Identitys.Update(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DomainEntities.IdentityId id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Identitys
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (entity != null)
        {
            _dbContext.Identitys.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    #region Pagination

    public async Task<PagedResult<Identity>> BrowseAsync(
        IPagedQuery query,
        CancellationToken cancellationToken = default)
    {
        return await BrowseAsync(query, c => true, cancellationToken);
    }

    public async Task<PagedResult<Identity>> BrowseAsync(
        IPagedQuery query,
        Expression<Func<Identity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await _dbContext.Identitys
            .Where(predicate)
            .CountAsync(cancellationToken);

        var items = await _dbContext.Identitys
            .Where(predicate)
            .Skip((query.Page - 1) * query.ResultsPerPage)
            .Take(query.ResultsPerPage)
            .ToListAsync(cancellationToken);

        // Process security attributes after loading (decrypt/hash verification)
        foreach (var entity in items)
        {
            ProcessSecurityAttributesFromStorage(entity);
        }

        return PagedResult<Identity>.Create(
            ImmutableList.CreateRange(items),
            query.Page,
            query.ResultsPerPage,
            (int)Math.Ceiling((double)totalCount / query.ResultsPerPage),
            totalCount);
    }

    #endregion

    #region Expression-Based Queries

    public async Task<IReadOnlyList<Identity>> FindAsync(
        Expression<Func<Identity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Identitys
            .Where(predicate)
            .ToListAsync(cancellationToken);
        
        // Process security attributes after loading (decrypt/hash verification)
        foreach (var entity in entities)
        {
            ProcessSecurityAttributesFromStorage(entity);
        }
        
        return ImmutableList.CreateRange(entities);
    }

    public async Task<Identity?> GetSingleOrDefaultAsync(
        Expression<Func<Identity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Identitys
            .Where(predicate)
            .SingleOrDefaultAsync(cancellationToken);
        
        if (entity != null)
        {
            // Process security attributes after loading (decrypt/hash verification)
            ProcessSecurityAttributesFromStorage(entity);
        }
        
        return entity;
    }

    public async Task<int> CountAsync(
        Expression<Func<Identity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        if (predicate == null)
        {
            return await _dbContext.Identitys.CountAsync(cancellationToken);
        }

        return await _dbContext.Identitys
            .Where(predicate)
            .CountAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<Identity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Identitys
            .AnyAsync(predicate, cancellationToken);
    }

    #endregion

    #region Bulk Operations

    public async Task AddRangeAsync(
        IEnumerable<Identity> entities,
        CancellationToken cancellationToken = default)
    {
        // Process security attributes before saving (encrypt/hash)
        foreach (var entity in entities)
        {
            ProcessSecurityAttributesToStorage(entity);
        }
        await _dbContext.Identitys.AddRangeAsync(entities, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(
        IEnumerable<Identity> entities,
        CancellationToken cancellationToken = default)
    {
        // Process security attributes before saving (encrypt/hash)
        foreach (var entity in entities)
        {
            ProcessSecurityAttributesToStorage(entity);
        }
        _dbContext.Identitys.UpdateRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRangeAsync(
        IEnumerable<DomainEntities.IdentityId> ids,
        CancellationToken cancellationToken = default)
    {
        var entities = await _dbContext.Identitys
            .Where(e => ids.Contains(e.Id))
            .ToListAsync(cancellationToken);

        if (entities.Any())
        {
            _dbContext.Identitys.RemoveRange(entities);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    #endregion

    #region Pessimistic Locking

    /// <summary>
    /// Gets an identity with an exclusive lock (SELECT FOR UPDATE).
    /// This method should be used within a transaction with Serializable isolation level.
    /// </summary>
    public async Task<Identity?> GetWithLockAsync(DomainEntities.IdentityId id, CancellationToken cancellationToken = default)
    {
        // Use raw SQL for SELECT FOR UPDATE to ensure exclusive lock
        var entity = await _dbContext.Identitys
            .FromSqlRaw("SELECT * FROM identity WHERE id = {0} FOR UPDATE", id.Value)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (entity != null)
        {
            // Process security attributes after loading (decrypt/hash verification)
            ProcessSecurityAttributesFromStorage(entity);
        }
        
        return entity;
    }

    /// <summary>
    /// Gets an identity matching the predicate with an exclusive lock (SELECT FOR UPDATE).
    /// This method should be used within a transaction with Serializable isolation level.
    /// </summary>
    public async Task<Identity?> GetSingleOrDefaultWithLockAsync(
        Expression<Func<Identity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        // Note: EF Core doesn't support SELECT FOR UPDATE directly in LINQ
        // For complex predicates, we need to use raw SQL or execute the query first
        // This is a simplified version - for production, consider using FromSqlRaw with parameterized queries
        
        // First, get the ID using the predicate
        var id = await _dbContext.Identitys
            .Where(predicate)
            .Select(e => e.Id)
            .FirstOrDefaultAsync(cancellationToken);
        
        if (id == null)
        {
            return null;
        }
        
        // Then use SELECT FOR UPDATE on the specific ID
        return await GetWithLockAsync(id, cancellationToken);
    }

    #endregion

    #region Security Processing

    /// <summary>
    /// Processes security attributes before saving to storage (encrypt/hash).
    /// </summary>
    private void ProcessSecurityAttributesToStorage(Identity entity)
    {
        if (entity == null) return;
        
        // Process entity itself
        _securityProcessor.ProcessAllSecurityAttributes(entity, ProcessingDirection.ToStorage);
        
        // Process nested value objects
        if (entity.BiometricData != null)
        {
            _securityProcessor.ProcessAllSecurityAttributes(entity.BiometricData, ProcessingDirection.ToStorage);
        }
    }

    /// <summary>
    /// Processes security attributes after loading from storage (decrypt/hash verification).
    /// </summary>
    private void ProcessSecurityAttributesFromStorage(Identity entity)
    {
        if (entity == null) return;
        
        // Process entity itself
        _securityProcessor.ProcessAllSecurityAttributes(entity, ProcessingDirection.FromStorage);
        
        // Process nested value objects
        if (entity.BiometricData != null)
        {
            _securityProcessor.ProcessAllSecurityAttributes(entity.BiometricData, ProcessingDirection.FromStorage);
        }
    }

    #endregion

    #region IDisposable

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    #region Authentication-specific queries

    public async Task<Identity?> GetByDidAsync(string did, CancellationToken cancellationToken = default)
    {
        // Note: Identity entity doesn't have a direct Did property
        // DIDs are typically resolved through the DID service, not stored directly on Identity
        // For now, we return null - DID lookup should be done via the DID service
        // TODO: Implement proper DID lookup once DID linking is added to Identity entity
        return await Task.FromResult<Identity?>(null);
    }

    public async Task<Identity?> GetByAzureUserIdAsync(string azureUserId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Identitys
            .Where(i => i.AzureUserId == azureUserId)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity != null)
        {
            ProcessSecurityAttributesFromStorage(entity);
        }

        return entity;
    }

    public async Task<Identity?> GetByServiceIdAsync(string serviceId, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Identitys
            .Where(i => i.ServiceId == serviceId)
            .SingleOrDefaultAsync(cancellationToken);

        if (entity != null)
        {
            ProcessSecurityAttributesFromStorage(entity);
        }

        return entity;
    }

    #endregion
}