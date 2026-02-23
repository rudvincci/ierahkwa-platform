using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Educators.Domain.Entities;
using Pupitre.Educators.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Educators.Infrastructure.PostgreSQL.Repositories;

internal class EducatorPostgresRepository : EFRepository<Educator, EducatorId>, IEducatorRepository, IDisposable
{
    private readonly EducatorsDbContext _serviceNameDbContext;
    public EducatorPostgresRepository(EducatorsDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<Educator>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Educator> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.Educators.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Educator> GetAsync(EducatorId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.Educators
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Educator entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.Educators.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(EducatorId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.Educators.Any(c => c.Id == id.Value));
    public async Task UpdateAsync(Educator entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.Educators.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Educator entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(EducatorId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.Educators.Single(c => c.Id == id.Value), cancellationToken);
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