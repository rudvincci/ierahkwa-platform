using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Compliance.Domain.Entities;
using Pupitre.Compliance.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Compliance.Infrastructure.EF.Repositories;

internal class ComplianceRecordPostgresRepository : EFRepository<ComplianceRecord, ComplianceRecordId>, IComplianceRecordRepository, IDisposable
{
    private readonly ComplianceRecordDbContext _entityNameDbContext;
    public ComplianceRecordPostgresRepository(ComplianceRecordDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<ComplianceRecord>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<ComplianceRecord> entityNames = ImmutableList.CreateRange(_entityNameDbContext.ComplianceRecords.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<ComplianceRecord> GetAsync(ComplianceRecordId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.ComplianceRecords
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(ComplianceRecord entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.ComplianceRecords.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(ComplianceRecordId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.ComplianceRecords.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(ComplianceRecord entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.ComplianceRecords.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(ComplianceRecord entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(ComplianceRecordId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.ComplianceRecords.Single(c => c.Id == id.Value), cancellationToken);
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