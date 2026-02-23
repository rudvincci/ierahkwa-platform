using System.Linq.Expressions;
using Common.Domain.Entities;

namespace Common.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    IQueryable<T> Query();
}

public interface ITenantRepository<T> : IRepository<T> where T : TenantEntity
{
    Task<IEnumerable<T>> GetAllForTenantAsync(int tenantId);
}
