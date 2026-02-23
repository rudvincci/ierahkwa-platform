using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Parents.Domain.Repositories;
using Pupitre.Parents.Domain.Entities;
using Pupitre.Parents.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Parents.Infrastructure.Mongo.Repositories;

internal class ParentMongoRepository : IParentRepository
{
    private readonly IMongoRepository<ParentDocument, Guid> _repository;

    public ParentMongoRepository(IMongoRepository<ParentDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Parent parent, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new ParentDocument(parent));

    public async Task UpdateAsync(Parent parent, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new ParentDocument(parent));
    public async Task DeleteAsync(ParentId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<Parent>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<Parent> GetAsync(ParentId id, CancellationToken cancellationToken = default)
    {
        var parent = await _repository.GetAsync(id.Value);
        return parent?.AsEntity();
    }
    public async Task<bool> ExistsAsync(ParentId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



