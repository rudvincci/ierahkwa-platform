using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Aftercare.Domain.Repositories;
using Pupitre.Aftercare.Domain.Entities;
using Pupitre.Aftercare.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Aftercare.Infrastructure.Mongo.Repositories;

internal class AftercarePlanMongoRepository : IAftercarePlanRepository
{
    private readonly IMongoRepository<AftercarePlanDocument, Guid> _repository;

    public AftercarePlanMongoRepository(IMongoRepository<AftercarePlanDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(AftercarePlan aftercareplan, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new AftercarePlanDocument(aftercareplan));

    public async Task UpdateAsync(AftercarePlan aftercareplan, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new AftercarePlanDocument(aftercareplan));
    public async Task DeleteAsync(AftercarePlanId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<AftercarePlan>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<AftercarePlan> GetAsync(AftercarePlanId id, CancellationToken cancellationToken = default)
    {
        var aftercareplan = await _repository.GetAsync(id.Value);
        return aftercareplan?.AsEntity();
    }
    public async Task<bool> ExistsAsync(AftercarePlanId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



