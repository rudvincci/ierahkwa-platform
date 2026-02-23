using Mamey.Persistence.SQL.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Mamey.Persistence.SQL;

/// <summary>
/// Clase genérica de Manager
/// </summary>
public class GenericManager<T> : IGenericManager<T>
    where T : class
{
    /// <summary>
    /// Manager data context
    /// </summary>
    public DbContext Context { get; private set; }

    /// <summary>
    /// Manager Contructor
    /// </summary>
    /// <param name="context">data context</param>
    public GenericManager(DbContext context)
    {
        Context = context;
    }

    /// <summary>
    /// Add entity to data context
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <returns>Added entity</returns>
    public T Add(T entity)
    {
        Context.Set<T>().Add(entity);
        Context.SaveChanges();
        return entity;
    }

    public async Task<T> Update(T entity)
    {
        Context.Set<T>().Update(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Add entity to data context async
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <returns>Added Entity</returns>
    public async Task<T> AddAsync(T entity)
    {
        await Context.Set<T>().AddAsync(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Remove entity from data context
    /// </summary>
    /// <param name="entity">Entity to remove</param>
    /// <returns>Entidad eliminada</returns>
    public async Task<T> Remove(T entity)
    {
        Context.Set<T>().Remove(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Get entity by id key
    /// </summary>
    /// <param name="key">id</param>
    /// <returns>Entity</returns>
    public T GetById(object[] key)
    {
        return Context.Set<T>().Find(key);
    }

    /// <summary>
    /// Get entity by his id
    /// </summary>
    /// <param name="id">id</param>
    /// <returns>Entity</returns>
    public T GetById(Guid id)
    {
        return GetById(new object[] {id});
    }

    /// <summary>
    /// Get all entities
    /// </summary>
    /// <returns>List of all entities</returns>
    public IQueryable<T> GetAll()
    {
        return Context.Set<T>();
    }
}