using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Educators.Domain.Repositories;
using Pupitre.Educators.Domain.Entities;
using Pupitre.Educators.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Educators.Infrastructure.Mongo.Repositories;

internal class EducatorMongoRepository : IEducatorRepository
{
    private readonly IMongoRepository<EducatorDocument, Guid> _repository;

    public EducatorMongoRepository(IMongoRepository<EducatorDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Educator educator, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new EducatorDocument(educator));

    public async Task UpdateAsync(Educator educator, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new EducatorDocument(educator));
    public async Task DeleteAsync(EducatorId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<Educator>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<Educator> GetAsync(EducatorId id, CancellationToken cancellationToken = default)
    {
        var educator = await _repository.GetAsync(id.Value);
        return educator?.AsEntity();
    }
    public async Task<bool> ExistsAsync(EducatorId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



