using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Compliance.Domain.Entities;
using Pupitre.Compliance.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Compliance.Infrastructure.PostgreSQL.Repositories;

internal class ComplianceRecordPostgresRepository : EFRepository<ComplianceRecord, ComplianceRecordId>, IComplianceRecordRepository, IDisposable
{
    private readonly ComplianceDbContext _serviceNameDbContext;
    public ComplianceRecordPostgresRepository(ComplianceDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<ComplianceRecord>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<ComplianceRecord> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.ComplianceRecords.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<ComplianceRecord> GetAsync(ComplianceRecordId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.ComplianceRecords
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(ComplianceRecord entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.ComplianceRecords.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(ComplianceRecordId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.ComplianceRecords.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(ComplianceRecord entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.ComplianceRecords.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(ComplianceRecord entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(ComplianceRecordId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.ComplianceRecords.Single(c => c.Id == id.Value), cancellationToken);
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