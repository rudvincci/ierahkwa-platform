using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Ministries.Domain.Entities;
using Pupitre.Ministries.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Ministries.Infrastructure.EF.Repositories;

internal class MinistryDataPostgresRepository : EFRepository<MinistryData, MinistryDataId>, IMinistryDataRepository, IDisposable
{
    private readonly MinistryDataDbContext _entityNameDbContext;
    public MinistryDataPostgresRepository(MinistryDataDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<MinistryData>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<MinistryData> entityNames = ImmutableList.CreateRange(_entityNameDbContext.MinistryDatas.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<MinistryData> GetAsync(MinistryDataId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.MinistryDatas
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(MinistryData entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.MinistryDatas.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(MinistryDataId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.MinistryDatas.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(MinistryData entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.MinistryDatas.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(MinistryData entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(MinistryDataId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.MinistryDatas.Single(c => c.Id == id.Value), cancellationToken);
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