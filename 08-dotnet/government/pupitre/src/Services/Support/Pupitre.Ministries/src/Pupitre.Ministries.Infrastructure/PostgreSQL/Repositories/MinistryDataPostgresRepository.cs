using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Ministries.Domain.Entities;
using Pupitre.Ministries.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Ministries.Infrastructure.PostgreSQL.Repositories;

internal class MinistryDataPostgresRepository : EFRepository<MinistryData, MinistryDataId>, IMinistryDataRepository, IDisposable
{
    private readonly MinistriesDbContext _serviceNameDbContext;
    public MinistryDataPostgresRepository(MinistriesDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<MinistryData>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<MinistryData> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.MinistryDatas.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<MinistryData> GetAsync(MinistryDataId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.MinistryDatas
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(MinistryData entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.MinistryDatas.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(MinistryDataId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.MinistryDatas.Any(c => c.Id == id.Value));
    public async Task UpdateAsync(MinistryData entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.MinistryDatas.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(MinistryData entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(MinistryDataId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.MinistryDatas.Single(c => c.Id == id.Value), cancellationToken);
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