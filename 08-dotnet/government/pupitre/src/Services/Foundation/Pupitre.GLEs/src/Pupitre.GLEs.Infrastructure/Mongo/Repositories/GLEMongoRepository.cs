using System;
using Mamey.Persistence.MongoDB;
using Pupitre.GLEs.Domain.Repositories;
using Pupitre.GLEs.Domain.Entities;
using Pupitre.GLEs.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.GLEs.Infrastructure.Mongo.Repositories;

internal class GLEMongoRepository : IGLERepository
{
    private readonly IMongoRepository<GLEDocument, Guid> _repository;

    public GLEMongoRepository(IMongoRepository<GLEDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(GLE gle, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new GLEDocument(gle));

    public async Task UpdateAsync(GLE gle, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new GLEDocument(gle));
    public async Task DeleteAsync(GLEId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<GLE>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<GLE> GetAsync(GLEId id, CancellationToken cancellationToken = default)
    {
        var gle = await _repository.GetAsync(id.Value);
        return gle?.AsEntity();
    }
    public async Task<bool> ExistsAsync(GLEId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



