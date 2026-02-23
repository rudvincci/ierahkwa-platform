using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AISafety.Domain.Entities;
using Pupitre.AISafety.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AISafety.Infrastructure.EF.Repositories;

internal class SafetyCheckPostgresRepository : EFRepository<SafetyCheck, SafetyCheckId>, ISafetyCheckRepository, IDisposable
{
    private readonly SafetyCheckDbContext _entityNameDbContext;
    public SafetyCheckPostgresRepository(SafetyCheckDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<SafetyCheck>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<SafetyCheck> entityNames = ImmutableList.CreateRange(_entityNameDbContext.SafetyChecks.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<SafetyCheck> GetAsync(SafetyCheckId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.SafetyChecks
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(SafetyCheck entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.SafetyChecks.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(SafetyCheckId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.SafetyChecks.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(SafetyCheck entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.SafetyChecks.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(SafetyCheck entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(SafetyCheckId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.SafetyChecks.Single(c => c.Id == id.Value), cancellationToken);
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