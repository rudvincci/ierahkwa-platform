using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Entities;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;
using Mamey.Government.Modules.TravelIdentities.Core.Mongo.Documents;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.TravelIdentities.Core.Mongo.Repositories;

internal class TravelIdentityMongoRepository : ITravelIdentityRepository
{
    private readonly IMongoRepository<TravelIdentityDocument, Guid> _repository;
    private readonly ILogger<TravelIdentityMongoRepository> _logger;

    public TravelIdentityMongoRepository(
        IMongoRepository<TravelIdentityDocument, Guid> repository,
        ILogger<TravelIdentityMongoRepository> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TravelIdentity?> GetAsync(TravelIdentityId id, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(id.Value);
        return document?.AsEntity();
    }

    public async Task AddAsync(TravelIdentity travelIdentity, CancellationToken cancellationToken = default)
    {
        var document = new TravelIdentityDocument(travelIdentity);
        await _repository.AddAsync(document);
    }

    public async Task UpdateAsync(TravelIdentity travelIdentity, CancellationToken cancellationToken = default)
    {
        var document = new TravelIdentityDocument(travelIdentity);
        await _repository.UpdateAsync(document);
    }

    public async Task DeleteAsync(TravelIdentityId id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id.Value);
    }

    public async Task<bool> ExistsAsync(TravelIdentityId id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(d => d.Id == id.Value);
    }

    public async Task<IReadOnlyList<TravelIdentity>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(_ => true);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<TravelIdentity?> GetByTravelIdentityNumberAsync(TravelIdentityNumber travelIdentityNumber, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(d => d.TravelIdentityNumber == travelIdentityNumber.Value);
        return document?.AsEntity();
    }

    public async Task<IReadOnlyList<TravelIdentity>> GetByCitizenAsync(CitizenId citizenId, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.CitizenId == citizenId.Value);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<TravelIdentity>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.TenantId == tenantId.Value);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public Task<int> GetCountByYearAsync(TenantId tenantId, int year, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
