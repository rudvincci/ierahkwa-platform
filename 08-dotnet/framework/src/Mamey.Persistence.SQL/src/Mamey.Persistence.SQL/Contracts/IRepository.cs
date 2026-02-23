using System.Linq.Expressions;
using Mamey.Types;

namespace Mamey.Persistence.SQL.Repositories;

public interface IEFRepository<TEntity, in TIdentifiable> where TEntity : IIdentifiable<TIdentifiable>
{
    Task<IEnumerable<TEntity>> GetAllAsync(bool noTracking = false, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(TIdentifiable id, CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Update(TEntity entity);
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    void Delete(TEntity entity);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = false, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, object>> orderBy, 
        bool ascending = true, CancellationToken cancellationToken = default);
    Task<TEntity?> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default);
    Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default);
    Task<TEntity?> MaxAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default);
    Task<TEntity?> MinAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default);
}
