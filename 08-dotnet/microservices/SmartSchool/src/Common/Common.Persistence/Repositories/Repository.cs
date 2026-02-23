using System.Linq.Expressions;
using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Common.Persistence.Repositories;

public class Repository<T, TContext> : IRepository<T> 
    where T : BaseEntity 
    where TContext : DbContext
{
    protected readonly TContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(TContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).Where(e => !e.IsDeleted).ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public virtual Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity)
    {
        entity.IsDeleted = true;
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id && !e.IsDeleted);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null)
            return await _dbSet.Where(e => !e.IsDeleted).CountAsync();
        return await _dbSet.Where(predicate).Where(e => !e.IsDeleted).CountAsync();
    }

    public virtual IQueryable<T> Query()
    {
        return _dbSet.Where(e => !e.IsDeleted).AsQueryable();
    }
}

public class TenantRepository<T, TContext> : Repository<T, TContext>, ITenantRepository<T> 
    where T : TenantEntity 
    where TContext : DbContext
{
    protected readonly ITenantService _tenantService;

    public TenantRepository(TContext context, ITenantService tenantService) : base(context)
    {
        _tenantService = tenantService;
    }

    public override async Task<IEnumerable<T>> GetAllAsync()
    {
        var tenantId = _tenantService.GetCurrentTenantId();
        if (tenantId == null)
            return await base.GetAllAsync();
        return await _dbSet.Where(e => e.TenantId == tenantId && !e.IsDeleted).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllForTenantAsync(int tenantId)
    {
        return await _dbSet.Where(e => e.TenantId == tenantId && !e.IsDeleted).ToListAsync();
    }

    public override async Task<T> AddAsync(T entity)
    {
        var tenantId = _tenantService.GetCurrentTenantId();
        if (tenantId != null)
            entity.TenantId = tenantId.Value;
        return await base.AddAsync(entity);
    }

    public override IQueryable<T> Query()
    {
        var tenantId = _tenantService.GetCurrentTenantId();
        if (tenantId == null)
            return base.Query();
        return _dbSet.Where(e => e.TenantId == tenantId && !e.IsDeleted).AsQueryable();
    }
}
