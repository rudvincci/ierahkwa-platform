using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AIAssessments.Domain.Entities;
using Pupitre.AIAssessments.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AIAssessments.Infrastructure.PostgreSQL.Repositories;

internal class AIAssessmentPostgresRepository : EFRepository<AIAssessment, AIAssessmentId>, IAIAssessmentRepository, IDisposable
{
    private readonly AIAssessmentsDbContext _serviceNameDbContext;
    public AIAssessmentPostgresRepository(AIAssessmentsDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<AIAssessment>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<AIAssessment> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.AIAssessments.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<AIAssessment> GetAsync(AIAssessmentId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.AIAssessments
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(AIAssessment entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.AIAssessments.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(AIAssessmentId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.AIAssessments.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(AIAssessment entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.AIAssessments.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(AIAssessment entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(AIAssessmentId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.AIAssessments.Single(c => c.Id == id.Value), cancellationToken);
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