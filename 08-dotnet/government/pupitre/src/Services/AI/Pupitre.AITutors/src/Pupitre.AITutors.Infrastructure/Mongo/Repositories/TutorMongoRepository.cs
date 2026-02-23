using System;
using Mamey.Persistence.MongoDB;
using Pupitre.AITutors.Domain.Repositories;
using Pupitre.AITutors.Domain.Entities;
using Pupitre.AITutors.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.AITutors.Infrastructure.Mongo.Repositories;

internal class TutorMongoRepository : ITutorRepository
{
    private readonly IMongoRepository<TutorDocument, Guid> _repository;

    public TutorMongoRepository(IMongoRepository<TutorDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Tutor tutor, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new TutorDocument(tutor));

    public async Task UpdateAsync(Tutor tutor, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new TutorDocument(tutor));
    public async Task DeleteAsync(TutorId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<Tutor>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<Tutor> GetAsync(TutorId id, CancellationToken cancellationToken = default)
    {
        var tutor = await _repository.GetAsync(id.Value);
        return tutor?.AsEntity();
    }
    public async Task<bool> ExistsAsync(TutorId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



