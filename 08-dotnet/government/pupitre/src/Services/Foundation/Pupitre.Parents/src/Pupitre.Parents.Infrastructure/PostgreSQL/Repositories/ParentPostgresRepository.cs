using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Parents.Domain.Entities;
using Pupitre.Parents.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Parents.Infrastructure.PostgreSQL.Repositories;

internal class ParentPostgresRepository : EFRepository<Parent, ParentId>, IParentRepository, IDisposable
{
    private readonly ParentsDbContext _serviceNameDbContext;
    public ParentPostgresRepository(ParentsDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<Parent>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Parent> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.Parents.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Parent> GetAsync(ParentId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.Parents
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Parent entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.Parents.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(ParentId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.Parents.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(Parent entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.Parents.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Parent entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(ParentId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.Parents.Single(c => c.Id == id.Value), cancellationToken);
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