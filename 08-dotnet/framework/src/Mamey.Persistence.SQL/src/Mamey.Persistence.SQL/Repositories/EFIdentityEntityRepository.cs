using System.Linq.Expressions;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Persistence.SQL.Repositories;

public class EFIdentityEntityRepository<TEntity, TIdentifiable> :
    EFRepository<TEntity, TIdentifiable>, IEFRepository<TEntity, TIdentifiable>
    where TEntity : EFIdentityEntity<TIdentifiable>, IIdentifiable<TIdentifiable>
    where TIdentifiable : IEquatable<TIdentifiable>
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public EFIdentityEntityRepository(DbContext context)
        : base(context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }
    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<TEntity> GetByIdAsync(TIdentifiable id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
        _context.SaveChanges();
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
        _context.SaveChanges();
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);

    }
    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, object>> orderBy, bool ascending = true)
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

        return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<TEntity> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.SingleOrDefaultAsync(predicate);
    }

    public async Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector)
    {
        return await _dbSet.SumAsync(selector);
    }

    public async Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector)
    {
        return await _dbSet.AverageAsync(selector);
    }

    public async Task<TEntity> MaxAsync(Expression<Func<TEntity, decimal>> selector)
    {
        return await _dbSet.OrderByDescending(selector)?.FirstOrDefaultAsync();
    }

    public async Task<TEntity> MinAsync(Expression<Func<TEntity, decimal>> selector)
    {
        return await _dbSet.OrderBy(selector).FirstOrDefaultAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}