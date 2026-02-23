using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Curricula.Domain.Repositories;
using Pupitre.Curricula.Domain.Entities;
using Pupitre.Curricula.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Curricula.Infrastructure.Mongo.Repositories;

internal class CurriculumMongoRepository : ICurriculumRepository
{
    private readonly IMongoRepository<CurriculumDocument, Guid> _repository;

    public CurriculumMongoRepository(IMongoRepository<CurriculumDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Curriculum curriculum, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new CurriculumDocument(curriculum));

    public async Task UpdateAsync(Curriculum curriculum, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new CurriculumDocument(curriculum));
    public async Task DeleteAsync(CurriculumId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<Curriculum>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<Curriculum> GetAsync(CurriculumId id, CancellationToken cancellationToken = default)
    {
        var curriculum = await _repository.GetAsync(id.Value);
        return curriculum?.AsEntity();
    }
    public async Task<bool> ExistsAsync(CurriculumId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



