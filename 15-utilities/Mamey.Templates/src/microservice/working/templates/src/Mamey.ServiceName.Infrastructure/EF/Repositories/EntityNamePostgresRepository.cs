using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Mamey.ServiceName.Domain.Entities;
using Mamey.ServiceName.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Mamey.ServiceName.Infrastructure.EF.Repositories;

internal class EntityNamePostgresRepository : EFRepository<EntityName, EntityNameId>, IEntityNameRepository, IDisposable
{
    private readonly EntityNameDbContext _entityNameDbContext;
    public EntityNamePostgresRepository(EntityNameDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<EntityName>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<EntityName> entityNames = ImmutableList.CreateRange(_entityNameDbContext.EntityNames.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<EntityName> GetAsync(EntityNameId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.EntityNames
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(EntityName entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.EntityNames.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(EntityNameId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.EntityNames.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(EntityName entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.EntityNames.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(EntityName entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(EntityNameId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.EntityNames.Single(c => c.Id == id.Value), cancellationToken);
    }
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _entityNameDbContext.Dispose();
            }
        }
        this.disposed = true;
    }
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}