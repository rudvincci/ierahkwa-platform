using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Mamey.CQRS.Queries;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Infrastructure.Mongo.Documents;
using Mamey.Persistence.MongoDB;
using Mamey.Types;
using DomainEntities = Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Infrastructure.Mongo.Repositories;

internal class IdentityMongoRepository : IIdentityRepository
{
    private readonly IMongoRepository<IdentityDocument, Guid> _repository;

    public IdentityMongoRepository(IMongoRepository<IdentityDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Identity entity, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new IdentityDocument(entity));

    public async Task UpdateAsync(Identity entity, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new IdentityDocument(entity));

    public async Task DeleteAsync(IdentityId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);

    public async Task<IReadOnlyList<Identity>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        // MongoDB is a read model - cannot convert back to full entities
        // Return empty list - composite repository will fall back to PostgreSQL
        return new List<Identity>();
    }

    public async Task<Identity?> GetAsync(IdentityId id, CancellationToken cancellationToken = default)
    {
        // MongoDB is a read model - cannot convert back to full entities
        // Return null - composite repository will fall back to PostgreSQL
        return null;
    }

    public async Task<bool> ExistsAsync(IdentityId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);

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
        // Convert Identity predicate to IdentityDocument predicate
        // For now, return all documents with pagination - complex expression translation would be needed
        var allDocuments = await _repository.FindAsync(_ => true);
        var totalCount = allDocuments.Count();
        var documents = allDocuments
            .Skip((query.Page - 1) * query.ResultsPerPage)
            .Take(query.ResultsPerPage)
            .ToList();

        // MongoDB is a read model - cannot convert back to full entities
        // Return empty result - composite repository will fall back to PostgreSQL
        return PagedResult<Identity>.Create(
            new List<Identity>(),
            query.Page,
            query.ResultsPerPage,
            0,
            0);
    }

    #endregion

    #region Expression-Based Queries

    public Task<IReadOnlyList<Identity>> FindAsync(
        Expression<Func<Identity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        // For complex queries, return empty - would need expression translation
        return Task.FromResult<IReadOnlyList<Identity>>(new List<Identity>());
    }

    public Task<Identity?> GetSingleOrDefaultAsync(
        Expression<Func<Identity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        // For complex queries, return null - would need expression translation
        return Task.FromResult<Identity?>(null);
    }

    public async Task<int> CountAsync(
        Expression<Func<Identity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var allDocuments = await _repository.FindAsync(_ => true);
        return allDocuments.Count();
    }

    public Task<bool> ExistsAsync(
        Expression<Func<Identity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        // For complex queries, return false - would need expression translation
        return Task.FromResult(false);
    }

    #endregion

    #region Bulk Operations

    public async Task AddRangeAsync(
        IEnumerable<Identity> entities,
        CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await _repository.AddAsync(new IdentityDocument(entity));
        }
    }

    public async Task UpdateRangeAsync(
        IEnumerable<Identity> entities,
        CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await _repository.UpdateAsync(new IdentityDocument(entity));
        }
    }

    public async Task DeleteRangeAsync(
        IEnumerable<IdentityId> ids,
        CancellationToken cancellationToken = default)
    {
        foreach (var id in ids)
        {
            await _repository.DeleteAsync(id.Value);
        }
    }

    #endregion

    #region Pessimistic Locking

    /// <summary>
    /// Gets an identity with an exclusive lock (SELECT FOR UPDATE).
    /// Note: MongoDB doesn't support pessimistic locking in the same way as PostgreSQL.
    /// This method returns null - use PostgreSQL repository for pessimistic locking.
    /// </summary>
    public Task<Identity?> GetWithLockAsync(IdentityId id, CancellationToken cancellationToken = default)
    {
        // MongoDB is a read model and doesn't support pessimistic locking
        // Pessimistic locking should be done on PostgreSQL (source of truth)
        return Task.FromResult<Identity?>(null);
    }

    /// <summary>
    /// Gets an identity matching the predicate with an exclusive lock (SELECT FOR UPDATE).
    /// Note: MongoDB doesn't support pessimistic locking in the same way as PostgreSQL.
    /// This method returns null - use PostgreSQL repository for pessimistic locking.
    /// </summary>
    public Task<Identity?> GetSingleOrDefaultWithLockAsync(
        Expression<Func<Identity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        // MongoDB is a read model and doesn't support pessimistic locking
        // Pessimistic locking should be done on PostgreSQL (source of truth)
        return Task.FromResult<Identity?>(null);
    }

    #endregion

    #region Authentication-specific queries

    public async Task<Identity?> GetByDidAsync(string did, CancellationToken cancellationToken = default)
    {
        // MongoDB is a read model - cannot convert back to full entities
        // Return null - composite repository will fall back to PostgreSQL
        return null;
    }

    public async Task<Identity?> GetByAzureUserIdAsync(string azureUserId, CancellationToken cancellationToken = default)
    {
        // MongoDB is a read model - cannot convert back to full entities
        // Return null - composite repository will fall back to PostgreSQL
        return null;
    }

    public async Task<Identity?> GetByServiceIdAsync(string serviceId, CancellationToken cancellationToken = default)
    {
        // MongoDB is a read model - cannot convert back to full entities
        // Return null - composite repository will fall back to PostgreSQL
        return null;
    }

    #endregion
}
