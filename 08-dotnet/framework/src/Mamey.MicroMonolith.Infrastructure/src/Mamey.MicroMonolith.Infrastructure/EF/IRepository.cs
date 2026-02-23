using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Types;
using Microsoft.EntityFrameworkCore;

namespace Mamey.MicroMonolith.Infrastructure.EF;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(object id);
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null);
    Task<PagedResult<TEntity>> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>>? filter = null);
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task SoftDeleteAsync(TEntity entity);
    Task HardDeleteAsync(TEntity entity);
    Task SaveChangesAsync();
}
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null)
    {
        return filter == null
            ? await _dbSet.ToListAsync()
            : await _dbSet.Where(filter).ToListAsync();
    }

    public async Task<PagedResult<TEntity>> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>>? filter = null)
    {
        IQueryable<TEntity> query = _dbSet;
        if (filter != null)
            query = query.Where(filter);
        var items = await query.ToListAsync();
        return PagedResult<TEntity>.Create(items, pageIndex, pageSize, items.Count()/pageSize, items.Count());
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public Task UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task SoftDeleteAsync(TEntity entity)
    {
        if (entity is ISoftDeletable softDeletable)
        {
            softDeletable.IsDeleted = true;
            softDeletable.DeletedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }
        return Task.CompletedTask;
    }

    public Task HardDeleteAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}

