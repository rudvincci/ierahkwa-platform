using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Mamey.ServiceName.Domain.Entities;
using Mamey.ServiceName.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Mamey.ServiceName.Infrastructure.PostgreSQL.Repositories;

internal class EntityNamePostgresRepository : EFRepository<EntityName, EntityNameId>, IEntityNameRepository, IDisposable
{
    private readonly ServiceNameDbContext _serviceNameDbContext;
    public EntityNamePostgresRepository(ServiceNameDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<EntityName>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<EntityName> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.EntityNames.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<EntityName> GetAsync(EntityNameId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.EntityNames
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(EntityName entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.EntityNames.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(EntityNameId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.EntityNames.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(EntityName entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.EntityNames.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(EntityName entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(EntityNameId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.EntityNames.Single(c => c.Id == id.Value), cancellationToken);
    }
    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _serviceNameDbContext.Dispose();
            }
        }
        this.disposed = true;
    }
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}