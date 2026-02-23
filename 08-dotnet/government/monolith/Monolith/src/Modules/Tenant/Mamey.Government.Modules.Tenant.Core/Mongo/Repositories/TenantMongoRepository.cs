using Mamey.Government.Modules.Tenant.Core.Domain.Entities;
using Mamey.Government.Modules.Tenant.Core.Domain.Repositories;
using Mamey.Government.Modules.Tenant.Core.Mongo.Documents;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Tenant.Core.Mongo.Repositories;

internal class TenantMongoRepository : ITenantRepository
{
    private readonly IMongoRepository<TenantDocument, Guid> _repository;
    private readonly ILogger<TenantMongoRepository> _logger;

    public TenantMongoRepository(
        IMongoRepository<TenantDocument, Guid> repository,
        ILogger<TenantMongoRepository> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TenantEntity?> GetAsync(TenantId id, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(id.Value);
        return document?.AsEntity();
    }

    public async Task AddAsync(TenantEntity tenant, CancellationToken cancellationToken = default)
    {
        var document = new TenantDocument(tenant);
        await _repository.AddAsync(document);
    }

    public async Task UpdateAsync(TenantEntity tenant, CancellationToken cancellationToken = default)
    {
        var document = new TenantDocument(tenant);
        await _repository.UpdateAsync(document);
    }

    public async Task DeleteAsync(TenantId id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id.Value);
    }

    public async Task<bool> ExistsAsync(TenantId id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(d => d.Id == id.Value);
    }

    public async Task<IReadOnlyList<TenantEntity>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(_ => true);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<TenantEntity?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(d => d.Domain == domain);
        return document?.AsEntity();
    }
}
