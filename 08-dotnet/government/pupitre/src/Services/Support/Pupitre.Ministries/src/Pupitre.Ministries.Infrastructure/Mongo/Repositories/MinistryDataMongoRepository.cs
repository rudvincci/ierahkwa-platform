using System;
using Mamey.Persistence.MongoDB;
using Pupitre.Ministries.Domain.Repositories;
using Pupitre.Ministries.Domain.Entities;
using Pupitre.Ministries.Infrastructure.Mongo.Documents;
using Mamey.Types;

namespace Pupitre.Ministries.Infrastructure.Mongo.Repositories;

internal class MinistryDataMongoRepository : IMinistryDataRepository
{
    private readonly IMongoRepository<MinistryDataDocument, Guid> _repository;

    public MinistryDataMongoRepository(IMongoRepository<MinistryDataDocument, Guid> repository)
    {
        _repository = repository;
    }

    public async Task AddAsync(MinistryData ministrydata, CancellationToken cancellationToken = default)
        => await _repository.AddAsync(new MinistryDataDocument(ministrydata));

    public async Task UpdateAsync(MinistryData ministrydata, CancellationToken cancellationToken = default)
        => await _repository.UpdateAsync(new MinistryDataDocument(ministrydata));
    public async Task DeleteAsync(MinistryDataId id, CancellationToken cancellationToken = default)
        => await _repository.DeleteAsync(id.Value);
    public async Task<IReadOnlyList<MinistryData>> BrowseAsync(CancellationToken cancellationToken = default)
        => (await _repository.FindAsync(_ => true))
        .Select(c => c.AsEntity())
        .ToList();
    public async Task<MinistryData> GetAsync(MinistryDataId id, CancellationToken cancellationToken = default)
    {
        var ministrydata = await _repository.GetAsync(id.Value);
        return ministrydata?.AsEntity();
    }
    public async Task<bool> ExistsAsync(MinistryDataId id, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(c => c.Id == id.Value);
}



