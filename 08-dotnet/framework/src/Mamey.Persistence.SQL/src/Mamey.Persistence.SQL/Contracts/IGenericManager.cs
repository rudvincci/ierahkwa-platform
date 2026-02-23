using Microsoft.EntityFrameworkCore;

namespace Mamey.Persistence.SQL.Contracts;

public interface IGenericManager<T> where T : class
{
    DbContext Context { get; }
    T Add(T entity);
    Task<T> AddAsync(T entity);
    IQueryable<T> GetAll();
    T GetById(object[] key);
    T GetById(Guid id);
    Task<T> Remove(T entity);
}