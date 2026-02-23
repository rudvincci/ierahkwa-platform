using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AIVision.Domain.Entities;
using Pupitre.AIVision.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AIVision.Infrastructure.PostgreSQL.Repositories;

internal class VisionAnalysisPostgresRepository : EFRepository<VisionAnalysis, VisionAnalysisId>, IVisionAnalysisRepository, IDisposable
{
    private readonly AIVisionDbContext _serviceNameDbContext;
    public VisionAnalysisPostgresRepository(AIVisionDbContext serviceNameDbContext)
        : base(serviceNameDbContext)
    {
        _serviceNameDbContext = serviceNameDbContext;
    }

    public Task<IReadOnlyList<VisionAnalysis>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<VisionAnalysis> entityNames = ImmutableList.CreateRange(_serviceNameDbContext.VisionAnalyses.ToList());

        return Task.FromResult(entityNames);
    }
    public Task<VisionAnalysis> GetAsync(VisionAnalysisId id, CancellationToken cancellationToken)
        => Task.FromResult(_serviceNameDbContext.VisionAnalyses
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(VisionAnalysis entityName, CancellationToken cancellationToken)
    {
        await _serviceNameDbContext.VisionAnalyses.AddAsync(entityName, cancellationToken);
        await _serviceNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(VisionAnalysisId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_serviceNameDbContext.VisionAnalyses.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(VisionAnalysis entityName, CancellationToken cancellationToken)
    {
        _serviceNameDbContext.VisionAnalyses.Update(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(VisionAnalysis entityName, CancellationToken cancellationToken = default)
    {
        _serviceNameDbContext.Remove(entityName);
        await _serviceNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(VisionAnalysisId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_serviceNameDbContext.VisionAnalyses.Single(c => c.Id == id.Value), cancellationToken);
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