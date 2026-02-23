using System;
using Mamey.Persistence.MongoDB;
using Pupitre.AIAdaptive.Domain.Repositories;
using Pupitre.AIAdaptive.Domain.Entities;
using Pupitre.AIAdaptive.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.AIAdaptive.Infrastructure.Mongo.Repositories;

internal class AdaptiveLearningMongoRepository : IAdaptiveLearningRepository
{
    private readonly IMongoRepository<AdaptiveLearningDocument, Guid> _repository;

    public AdaptiveLearningMongoRepository(IMongoRepository<AdaptiveLearningDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(AdaptiveLearning adaptivelearning, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new AdaptiveLearningDocument(adaptivelearning));

    public async Task UpdateAsync(AdaptiveLearning adaptivelearning, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new AdaptiveLearningDocument(adaptivelearning));
    public async Task DeleteAsync(AdaptiveLearningId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<AdaptiveLearning>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<AdaptiveLearning> GetAsync(AdaptiveLearningId id, CancellationToken cancellationToken = default)
    {
        var adaptivelearning = await _repository.GetAsync(id.Value);
        return adaptivelearning?.AsEntity();
    }
    public async Task<bool> ExistsAsync(AdaptiveLearningId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



