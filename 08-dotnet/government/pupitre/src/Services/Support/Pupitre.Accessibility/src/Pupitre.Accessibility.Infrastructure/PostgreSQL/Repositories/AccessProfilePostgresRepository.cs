using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Accessibility.Domain.Entities;
using Pupitre.Accessibility.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Accessibility.Infrastructure.PostgreSQL.Repositories;

internal class AccessProfilePostgresRepository : EFRepository<AccessProfile, AccessProfileId>, IAccessProfileRepository, IDisposable
{
    private readonly AccessibilityDbContext _serviceNameDbContext;
    public AccessProfilePostgresRepository(AccessibilityDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<AccessProfile>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<AccessProfile> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.AccessProfiles.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<AccessProfile> GetAsync(AccessProfileId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.AccessProfiles
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(AccessProfile entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.AccessProfiles.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(AccessProfileId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.AccessProfiles.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(AccessProfile entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.AccessProfiles.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(AccessProfile entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(AccessProfileId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.AccessProfiles.Single(c => c.Id == id.Value), cancellationToken);
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