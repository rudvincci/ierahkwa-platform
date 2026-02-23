using System;
using Mamey.Persistence.MongoDB;
using Pupitre.AISafety.Domain.Repositories;
using Pupitre.AISafety.Domain.Entities;
using Pupitre.AISafety.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.AISafety.Infrastructure.Mongo.Repositories;

internal class SafetyCheckMongoRepository : ISafetyCheckRepository
{
    private readonly IMongoRepository<SafetyCheckDocument, Guid> _repository;

    public SafetyCheckMongoRepository(IMongoRepository<SafetyCheckDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(SafetyCheck safetycheck, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new SafetyCheckDocument(safetycheck));

    public async Task UpdateAsync(SafetyCheck safetycheck, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new SafetyCheckDocument(safetycheck));
    public async Task DeleteAsync(SafetyCheckId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<SafetyCheck>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<SafetyCheck> GetAsync(SafetyCheckId id, CancellationToken cancellationToken = default)
    {
        var safetycheck = await _repository.GetAsync(id.Value);
        return safetycheck?.AsEntity();
    }
    public async Task<bool> ExistsAsync(SafetyCheckId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



