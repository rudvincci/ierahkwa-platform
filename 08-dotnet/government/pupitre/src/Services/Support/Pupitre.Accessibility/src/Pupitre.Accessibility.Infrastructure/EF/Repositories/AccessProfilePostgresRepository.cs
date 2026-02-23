using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Accessibility.Domain.Entities;
using Pupitre.Accessibility.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Accessibility.Infrastructure.EF.Repositories;

internal class AccessProfilePostgresRepository : EFRepository<AccessProfile, AccessProfileId>, IAccessProfileRepository, IDisposable
{
    private readonly AccessProfileDbContext _entityNameDbContext;
    public AccessProfilePostgresRepository(AccessProfileDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<AccessProfile>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<AccessProfile> entityNames = ImmutableList.CreateRange(_entityNameDbContext.AccessProfiles.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<AccessProfile> GetAsync(AccessProfileId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.AccessProfiles
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(AccessProfile entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.AccessProfiles.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(AccessProfileId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.AccessProfiles.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(AccessProfile entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.AccessProfiles.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(AccessProfile entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(AccessProfileId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.AccessProfiles.Single(c => c.Id == id.Value), cancellationToken);
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