using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Parents.Domain.Entities;
using Pupitre.Parents.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Parents.Infrastructure.EF.Repositories;

internal class ParentPostgresRepository : EFRepository<Parent, ParentId>, IParentRepository, IDisposable
{
    private readonly ParentDbContext _entityNameDbContext;
    public ParentPostgresRepository(ParentDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<Parent>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Parent> entityNames = ImmutableList.CreateRange(_entityNameDbContext.Parents.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Parent> GetAsync(ParentId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.Parents
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Parent entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.Parents.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(ParentId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.Parents.Any(c => c.Id == id.Value));
    public async Task UpdateAsync(Parent entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.Parents.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Parent entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(ParentId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.Parents.Single(c => c.Id == id.Value), cancellationToken);
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