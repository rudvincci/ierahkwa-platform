using System;
using Mamey.Persistence.MongoDB;
using Pupitre.AIBehavior.Domain.Repositories;
using Pupitre.AIBehavior.Domain.Entities;
using Pupitre.AIBehavior.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.AIBehavior.Infrastructure.Mongo.Repositories;

internal class BehaviorMongoRepository : IBehaviorRepository
{
    private readonly IMongoRepository<BehaviorDocument, Guid> _repository;

    public BehaviorMongoRepository(IMongoRepository<BehaviorDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(Behavior behavior, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new BehaviorDocument(behavior));

    public async Task UpdateAsync(Behavior behavior, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new BehaviorDocument(behavior));
    public async Task DeleteAsync(BehaviorId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<Behavior>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<Behavior> GetAsync(BehaviorId id, CancellationToken cancellationToken = default)
    {
        var behavior = await _repository.GetAsync(id.Value);
        return behavior?.AsEntity();
    }
    public async Task<bool> ExistsAsync(BehaviorId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



