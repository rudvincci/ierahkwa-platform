using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Assessments.Domain.Repositories;
using Pupitre.Assessments.Domain.Entities;
using Pupitre.Assessments.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Assessments.Infrastructure.Mongo.Repositories;

internal class AssessmentMongoRepository : IAssessmentRepository
{
    private readonly IMongoRepository<AssessmentDocument, Guid> _repository;

    public AssessmentMongoRepository(IMongoRepository<AssessmentDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Assessment assessment, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new AssessmentDocument(assessment));

    public async Task UpdateAsync(Assessment assessment, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new AssessmentDocument(assessment));
    public async Task DeleteAsync(AssessmentId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<Assessment>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<Assessment> GetAsync(AssessmentId id, CancellationToken cancellationToken = default)
    {
        var assessment = await _repository.GetAsync(id.Value);
        return assessment?.AsEntity();
    }
    public async Task<bool> ExistsAsync(AssessmentId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



