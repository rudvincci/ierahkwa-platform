using Microsoft.EntityFrameworkCore.Storage;
using Mamey.Auth.Decentralized.Persistence.Write.Entities;

namespace Mamey.Auth.Decentralized.Persistence.Write;

/// <summary>
/// Unit of Work interface for DID operations
/// </summary>
public interface IDidUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the DID Documents repository
    /// </summary>
    IRepository<DidDocumentEntity> DidDocuments { get; }

    /// <summary>
    /// Gets the Verification Methods repository
    /// </summary>
    IRepository<VerificationMethodEntity> VerificationMethods { get; }

    /// <summary>
    /// Gets the Service Endpoints repository
    /// </summary>
    IRepository<ServiceEndpointEntity> ServiceEndpoints { get; }

    /// <summary>
    /// Saves all changes
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of affected records</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a transaction
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The transaction</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Generic repository interface
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>All entities</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an entity by ID
    /// </summary>
    /// <param name="id">The entity ID</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The entity or null if not found</returns>
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities by a predicate
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Matching entities</returns>
    Task<IEnumerable<TEntity>> GetWhereAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an entity
    /// </summary>
    /// <param name="entity">The entity to add</param>
    void Add(TEntity entity);

    /// <summary>
    /// Adds multiple entities
    /// </summary>
    /// <param name="entities">The entities to add</param>
    void AddRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Updates an entity
    /// </summary>
    /// <param name="entity">The entity to update</param>
    void Update(TEntity entity);

    /// <summary>
    /// Updates multiple entities
    /// </summary>
    /// <param name="entities">The entities to update</param>
    void UpdateRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Removes an entity
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    void Remove(TEntity entity);

    /// <summary>
    /// Removes multiple entities
    /// </summary>
    /// <param name="entities">The entities to remove</param>
    void RemoveRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Checks if an entity exists
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the entity exists, false otherwise</returns>
    Task<bool> ExistsAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts entities matching a predicate
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The count</returns>
    Task<int> CountAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default);
}
