using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.GLEs.Domain.Entities;
using Pupitre.GLEs.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.GLEs.Infrastructure.PostgreSQL.Repositories;

internal class GLEPostgresRepository : EFRepository<GLE, GLEId>, IGLERepository, IDisposable
{
    private readonly GLEsDbContext _serviceNameDbContext;
    public GLEPostgresRepository(GLEsDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<GLE>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<GLE> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.GLEs.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<GLE> GetAsync(GLEId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.GLEs
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(GLE entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.GLEs.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(GLEId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.GLEs.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(GLE entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.GLEs.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(GLE entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(GLEId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.GLEs.Single(c => c.Id == id.Value), cancellationToken);
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