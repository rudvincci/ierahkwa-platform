using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Progress.Domain.Repositories;
using Pupitre.Progress.Domain.Entities;
using Pupitre.Progress.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Progress.Infrastructure.Mongo.Repositories;

internal class LearningProgressMongoRepository : ILearningProgressRepository
{
    private readonly IMongoRepository<LearningProgressDocument, Guid> _repository;

    public LearningProgressMongoRepository(IMongoRepository<LearningProgressDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(LearningProgress learningprogress, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new LearningProgressDocument(learningprogress));

    public async Task UpdateAsync(LearningProgress learningprogress, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new LearningProgressDocument(learningprogress));
    public async Task DeleteAsync(LearningProgressId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<LearningProgress>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<LearningProgress> GetAsync(LearningProgressId id, CancellationToken cancellationToken = default)
    {
        var learningprogress = await _repository.GetAsync(id.Value);
        return learningprogress?.AsEntity();
    }
    public async Task<bool> ExistsAsync(LearningProgressId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



