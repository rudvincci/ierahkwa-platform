using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.GLEs.Domain.Entities;
using Pupitre.GLEs.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.GLEs.Infrastructure.EF.Repositories;

internal class GLEPostgresRepository : EFRepository<GLE, GLEId>, IGLERepository, IDisposable
{
    private readonly GLEDbContext _entityNameDbContext;
    public GLEPostgresRepository(GLEDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<GLE>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<GLE> entityNames = ImmutableList.CreateRange(_entityNameDbContext.GLEs.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<GLE> GetAsync(GLEId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.GLEs
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(GLE entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.GLEs.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(GLEId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.GLEs.Any(c => c.Id == id.Value));
    public async Task UpdateAsync(GLE entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.GLEs.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(GLE entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(GLEId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.GLEs.Single(c => c.Id == id.Value), cancellationToken);
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