using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Persistence.SQL.Repositories;


public class EFRepository<TEntity, TIdentifiable> :
    IEFRepository<TEntity, TIdentifiable>
    where TEntity : class, IIdentifiable<TIdentifiable>
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public EFRepository(DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    #region Methods

    public async Task<IEnumerable<TEntity>> GetAllAsync(bool noTracking = false, CancellationToken cancellationToken = default)
    {
        if (noTracking)
        {
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
        }
        return await _dbSet.ToListAsync(cancellationToken);
    }

    

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
        _context.SaveChanges();
    }
    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
        _context.SaveChanges();
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking = false, CancellationToken cancellationToken = default)
    {
        if (noTracking)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, object>> orderBy, 
        bool ascending = true, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        if (ascending)
        {
            query = query.OrderBy(orderBy);
        }
        else
        {
            query = query.OrderByDescending(orderBy);
        }

        return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<TEntity?> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default)
    {
        return await _dbSet.SumAsync(selector, cancellationToken);
    }

    public async Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AverageAsync(selector, cancellationToken);
    }

    public async Task<TEntity?> MaxAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default)
    {
        return await _dbSet.OrderByDescending(selector).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TEntity?> MinAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default)
    {
        return await _dbSet.OrderBy(selector).FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdAsync([DisallowNull]TIdentifiable id, CancellationToken cancellationToken = default)
    {
        if (id == null) throw new ArgumentNullException(nameof(id));
        return await _dbSet.FindAsync(id, cancellationToken);
    }
    #endregion
    
}