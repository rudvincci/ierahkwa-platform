using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AISafety.Domain.Entities;
using Pupitre.AISafety.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AISafety.Infrastructure.PostgreSQL.Repositories;

internal class SafetyCheckPostgresRepository : EFRepository<SafetyCheck, SafetyCheckId>, ISafetyCheckRepository, IDisposable
{
    private readonly AISafetyDbContext _serviceNameDbContext;
    public SafetyCheckPostgresRepository(AISafetyDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<SafetyCheck>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<SafetyCheck> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.SafetyChecks.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<SafetyCheck> GetAsync(SafetyCheckId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.SafetyChecks
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(SafetyCheck entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.SafetyChecks.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(SafetyCheckId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.SafetyChecks.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(SafetyCheck entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.SafetyChecks.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(SafetyCheck entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(SafetyCheckId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.SafetyChecks.Single(c => c.Id == id.Value), cancellationToken);
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