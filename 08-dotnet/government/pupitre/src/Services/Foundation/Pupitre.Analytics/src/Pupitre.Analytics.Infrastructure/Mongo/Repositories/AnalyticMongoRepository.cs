using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Analytics.Domain.Repositories;
using Pupitre.Analytics.Domain.Entities;
using Pupitre.Analytics.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Analytics.Infrastructure.Mongo.Repositories;

internal class AnalyticMongoRepository : IAnalyticRepository
{
    private readonly IMongoRepository<AnalyticDocument, Guid> _repository;

    public AnalyticMongoRepository(IMongoRepository<AnalyticDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Analytic analytic, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new AnalyticDocument(analytic));

    public async Task UpdateAsync(Analytic analytic, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new AnalyticDocument(analytic));
    public async Task DeleteAsync(AnalyticId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<Analytic>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<Analytic> GetAsync(AnalyticId id, CancellationToken cancellationToken = default)
    {
        var analytic = await _repository.GetAsync(id.Value);
        return analytic?.AsEntity();
    }
    public async Task<bool> ExistsAsync(AnalyticId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



