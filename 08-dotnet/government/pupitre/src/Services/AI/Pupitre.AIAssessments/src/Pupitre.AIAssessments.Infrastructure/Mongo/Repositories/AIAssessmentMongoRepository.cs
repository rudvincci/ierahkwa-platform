using System;
using Mamey.Persistence.MongoDB;
using Pupitre.AIAssessments.Domain.Repositories;
using Pupitre.AIAssessments.Domain.Entities;
using Pupitre.AIAssessments.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.AIAssessments.Infrastructure.Mongo.Repositories;

internal class AIAssessmentMongoRepository : IAIAssessmentRepository
{
    private readonly IMongoRepository<AIAssessmentDocument, Guid> _repository;

    public AIAssessmentMongoRepository(IMongoRepository<AIAssessmentDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(AIAssessment aiassessment, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new AIAssessmentDocument(aiassessment));

    public async Task UpdateAsync(AIAssessment aiassessment, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new AIAssessmentDocument(aiassessment));
    public async Task DeleteAsync(AIAssessmentId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<AIAssessment>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<AIAssessment> GetAsync(AIAssessmentId id, CancellationToken cancellationToken = default)
    {
        var aiassessment = await _repository.GetAsync(id.Value);
        return aiassessment?.AsEntity();
    }
    public async Task<bool> ExistsAsync(AIAssessmentId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



