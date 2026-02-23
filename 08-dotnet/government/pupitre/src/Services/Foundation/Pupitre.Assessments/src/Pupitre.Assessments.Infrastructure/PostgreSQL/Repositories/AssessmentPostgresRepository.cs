using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.Assessments.Domain.Entities;
using Pupitre.Assessments.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.Assessments.Infrastructure.PostgreSQL.Repositories;

internal class AssessmentPostgresRepository : EFRepository<Assessment, AssessmentId>, IAssessmentRepository, IDisposable
{
    private readonly AssessmentsDbContext _serviceNameDbContext;
    public AssessmentPostgresRepository(AssessmentsDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<Assessment>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Assessment> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.Assessments.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<Assessment> GetAsync(AssessmentId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.Assessments
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(Assessment entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.Assessments.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(AssessmentId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.Assessments.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(Assessment entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.Assessments.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(Assessment entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(AssessmentId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.Assessments.Single(c => c.Id == id.Value), cancellationToken);
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