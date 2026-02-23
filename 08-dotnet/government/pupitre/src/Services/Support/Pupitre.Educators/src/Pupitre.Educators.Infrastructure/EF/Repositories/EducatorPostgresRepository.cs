using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Educators.Domain.Entities;
using Pupitre.Educators.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Educators.Infrastructure.EF.Repositories;

internal class EducatorPostgresRepository : EFRepository<Educator, EducatorId>, IEducatorRepository, IDisposable
{
    private readonly EducatorDbContext _entityNameDbContext;
    public EducatorPostgresRepository(EducatorDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<Educator>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Educator> entityNames = ImmutableList.CreateRange(_entityNameDbContext.Educators.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Educator> GetAsync(EducatorId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.Educators
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Educator entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.Educators.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(EducatorId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.Educators.Any(c => c.Id == id.Value));
    public async Task UpdateAsync(Educator entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.Educators.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Educator entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(EducatorId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.Educators.Single(c => c.Id == id.Value), cancellationToken);
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