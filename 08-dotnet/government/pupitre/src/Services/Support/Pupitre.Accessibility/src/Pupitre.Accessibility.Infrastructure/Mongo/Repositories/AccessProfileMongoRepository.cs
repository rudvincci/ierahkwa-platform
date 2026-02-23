using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Accessibility.Domain.Repositories;
using Pupitre.Accessibility.Domain.Entities;
using Pupitre.Accessibility.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Accessibility.Infrastructure.Mongo.Repositories;

internal class AccessProfileMongoRepository : IAccessProfileRepository
{
    private readonly IMongoRepository<AccessProfileDocument, Guid> _repository;

    public AccessProfileMongoRepository(IMongoRepository<AccessProfileDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(AccessProfile accessprofile, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new AccessProfileDocument(accessprofile));

    public async Task UpdateAsync(AccessProfile accessprofile, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new AccessProfileDocument(accessprofile));
    public async Task DeleteAsync(AccessProfileId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<AccessProfile>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<AccessProfile> GetAsync(AccessProfileId id, CancellationToken cancellationToken = default)
    {
        var accessprofile = await _repository.GetAsync(id.Value);
        return accessprofile?.AsEntity();
    }
    public async Task<bool> ExistsAsync(AccessProfileId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



