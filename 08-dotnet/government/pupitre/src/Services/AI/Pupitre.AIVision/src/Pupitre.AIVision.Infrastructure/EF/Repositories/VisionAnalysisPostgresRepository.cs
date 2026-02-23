using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Mamey.Persistence.SQL.Repositories;
using Pupitre.AIVision.Domain.Entities;
using Pupitre.AIVision.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Pupitre.AIVision.Infrastructure.EF.Repositories;

internal class VisionAnalysisPostgresRepository : EFRepository<VisionAnalysis, VisionAnalysisId>, IVisionAnalysisRepository, IDisposable
{
    private readonly VisionAnalysisDbContext _entityNameDbContext;
    public VisionAnalysisPostgresRepository(VisionAnalysisDbContext entityNameDbContext)
        : base(entityNameDbContext)
    {
        _entityNameDbContext = entityNameDbContext;
    }

    public Task<IReadOnlyList<VisionAnalysis>> BrowseAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<VisionAnalysis> entityNames = ImmutableList.CreateRange(_entityNameDbContext.VisionAnalyses.ToList()
            .ToList());

        return Task.FromResult(entityNames);
    }
    public Task<VisionAnalysis> GetAsync(VisionAnalysisId id, CancellationToken cancellationToken)
        => Task.FromResult(_entityNameDbContext.VisionAnalyses
            .Where(c => c.Id == id.Value)
            .Single());

    public async Task AddAsync(VisionAnalysis entityName, CancellationToken cancellationToken)
    {
        await _entityNameDbContext.VisionAnalyses.AddAsync(entityName, cancellationToken);
        await _entityNameDbContext.SaveChangesAsync();
    }

    public Task<bool> ExistsAsync(VisionAnalysisId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_entityNameDbContext.VisionAnalyses.Any(c => c.Id == c.Id));
    public async Task UpdateAsync(VisionAnalysis entityName, CancellationToken cancellationToken)
    {
        _entityNameDbContext.VisionAnalyses.Update(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public async Task DeleteAsync(VisionAnalysis entityName, CancellationToken cancellationToken = default)
    {
        _entityNameDbContext.Remove(entityName);
        await _entityNameDbContext.SaveChangesAsync();
    }
    public Task DeleteAsync(VisionAnalysisId id, CancellationToken cancellationToken = default)
    {
        return DeleteAsync(_entityNameDbContext.VisionAnalyses.Single(c => c.Id == id.Value), cancellationToken);
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