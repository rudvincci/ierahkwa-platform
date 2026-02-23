using Mamey.Government.Modules.Citizens.Core.Domain.Entities;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.Mongo.Documents;
using Mamey.Types;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Citizens.Core.Mongo.Repositories;

internal class CitizenMongoRepository : ICitizenRepository
{
    private readonly IMongoRepository<CitizenDocument, Guid> _repository;
    private readonly ILogger<CitizenMongoRepository> _logger;

    public CitizenMongoRepository(
        IMongoRepository<CitizenDocument, Guid> repository,
        ILogger<CitizenMongoRepository> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Citizen?> GetAsync(CitizenId id, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(id.Value);
        return document?.AsEntity();
    }

    public async Task AddAsync(Citizen citizen, CancellationToken cancellationToken = default)
    {
        var document = new CitizenDocument(citizen);
        await _repository.AddAsync(document);
    }

    public async Task UpdateAsync(Citizen citizen, CancellationToken cancellationToken = default)
    {
        var document = new CitizenDocument(citizen);
        await _repository.UpdateAsync(document);
    }

    public async Task DeleteAsync(CitizenId id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id.Value);
    }

    public async Task<bool> ExistsAsync(CitizenId id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(d => d.Id == id.Value);
    }

    public async Task<IReadOnlyList<Citizen>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(_ => true);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Citizen>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.TenantId == tenantId.Value);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Citizen>> GetByStatusAsync(CitizenshipStatus status, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.Status == status.ToString());
        return documents.Select(d => d.AsEntity()).ToList();
    }
}
