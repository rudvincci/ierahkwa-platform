using System;
using Mamey.Persistence.MongoDB;
using Mamey.ServiceName.Domain.Repositories;
using Mamey.ServiceName.Domain.Entities;
using Mamey.ServiceName.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Mamey.ServiceName.Infrastructure.Mongo.Repositories;

internal class EntityNameMongoRepository : IEntityNameRepository
{
    private readonly IMongoRepository<EntityNameDocument, Guid> _repository;

    public EntityNameMongoRepository(IMongoRepository<EntityNameDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(EntityName entityname, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new EntityNameDocument(entityname));

    public async Task UpdateAsync(EntityName entityname, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new EntityNameDocument(entityname));
    public async Task DeleteAsync(EntityNameId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<EntityName>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<EntityName> GetAsync(EntityNameId id, CancellationToken cancellationToken = default)
    {
        var entityname = await _repository.GetAsync(id.Value);
        return entityname?.AsEntity();
    }
    public async Task<bool> ExistsAsync(EntityNameId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



