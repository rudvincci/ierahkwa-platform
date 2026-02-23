using Microsoft.EntityFrameworkCore.Storage;
using Mamey.Auth.Decentralized.Persistence.Write.Entities;

namespace Mamey.Auth.Decentralized.Persistence.Write;

/// <summary>
/// Unit of Work implementation for DID operations
/// </summary>
public class DidUnitOfWork : IDidUnitOfWork
{
    private readonly DidDbContext _context;
    private IDbContextTransaction? _transaction;

    /// <summary>
    /// Initializes a new instance of the DidUnitOfWork class
    /// </summary>
    /// <param name="context">The DbContext</param>
    public DidUnitOfWork(DidDbContext context)
    {
        _context = context;
        DidDocuments = new Repository<DidDocumentEntity>(_context);
        VerificationMethods = new Repository<VerificationMethodEntity>(_context);
        ServiceEndpoints = new Repository<ServiceEndpointEntity>(_context);
    }

    /// <summary>
    /// Gets the DID Documents repository
    /// </summary>
    public IRepository<DidDocumentEntity> DidDocuments { get; }

    /// <summary>
    /// Gets the Verification Methods repository
    /// </summary>
    public IRepository<VerificationMethodEntity> VerificationMethods { get; }

    /// <summary>
    /// Gets the Service Endpoints repository
    /// </summary>
    public IRepository<ServiceEndpointEntity> ServiceEndpoints { get; }

    /// <summary>
    /// Saves all changes
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The number of affected records</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Begins a transaction
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The transaction</returns>
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return _transaction;
    }

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Disposes the unit of work
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

/// <summary>
/// Generic repository implementation
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DidDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    /// <summary>
    /// Initializes a new instance of the Repository class
    /// </summary>
    /// <param name="context">The DbContext</param>
    public Repository(DidDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>All entities</returns>
    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets an entity by ID
    /// </summary>
    /// <param name="id">The entity ID</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The entity or null if not found</returns>
    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    /// <summary>
    /// Gets entities by a predicate
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>Matching entities</returns>
    public async Task<IEnumerable<TEntity>> GetWhereAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => _dbSet.Where(predicate).ToList(), cancellationToken);
    }

    /// <summary>
    /// Adds an entity
    /// </summary>
    /// <param name="entity">The entity to add</param>
    public void Add(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    /// <summary>
    /// Adds multiple entities
    /// </summary>
    /// <param name="entities">The entities to add</param>
    public void AddRange(IEnumerable<TEntity> entities)
    {
        _dbSet.AddRange(entities);
    }

    /// <summary>
    /// Updates an entity
    /// </summary>
    /// <param name="entity">The entity to update</param>
    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    /// <summary>
    /// Updates multiple entities
    /// </summary>
    /// <param name="entities">The entities to update</param>
    public void UpdateRange(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    /// <summary>
    /// Removes an entity
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    public void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    /// <summary>
    /// Removes multiple entities
    /// </summary>
    /// <param name="entities">The entities to remove</param>
    public void RemoveRange(IEnumerable<TEntity> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    /// <summary>
    /// Checks if an entity exists
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>True if the entity exists, false otherwise</returns>
    public async Task<bool> ExistsAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => _dbSet.Any(predicate), cancellationToken);
    }

    /// <summary>
    /// Counts entities matching a predicate
    /// </summary>
    /// <param name="predicate">The predicate</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The count</returns>
    public async Task<int> CountAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => _dbSet.Count(predicate), cancellationToken);
    }
}
