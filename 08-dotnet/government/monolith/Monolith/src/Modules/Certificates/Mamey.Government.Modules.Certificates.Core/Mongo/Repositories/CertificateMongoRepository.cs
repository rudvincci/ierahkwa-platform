using Mamey.Government.Modules.Certificates.Core.Domain.Entities;
using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Certificates.Core.Mongo.Documents;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Types;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Certificates.Core.Mongo.Repositories;

internal class CertificateMongoRepository : ICertificateRepository
{
    private readonly IMongoRepository<CertificateDocument, Guid> _repository;
    private readonly ILogger<CertificateMongoRepository> _logger;

    public CertificateMongoRepository(
        IMongoRepository<CertificateDocument, Guid> repository,
        ILogger<CertificateMongoRepository> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Certificate?> GetAsync(CertificateId id, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(id.Value);
        return document?.AsEntity();
    }

    public async Task AddAsync(Certificate certificate, CancellationToken cancellationToken = default)
    {
        var document = new CertificateDocument(certificate);
        await _repository.AddAsync(document);
    }

    public async Task UpdateAsync(Certificate certificate, CancellationToken cancellationToken = default)
    {
        var document = new CertificateDocument(certificate);
        await _repository.UpdateAsync(document);
    }

    public async Task DeleteAsync(CertificateId id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id.Value);
    }

    public async Task<bool> ExistsAsync(CertificateId id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(d => d.Id == id.Value);
    }

    public async Task<IReadOnlyList<Certificate>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(_ => true);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<Certificate?> GetByCertificateNumberAsync(string certificateNumber, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(d => d.CertificateNumber == certificateNumber);
        return document?.AsEntity();
    }

    public async Task<IReadOnlyList<Certificate>> GetByCitizenAsync(CitizenId citizenId, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.CitizenId == citizenId.Value);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Certificate>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.TenantId == tenantId.Value);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Certificate>> GetByTypeAsync(CertificateType certificateType, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.CertificateType == certificateType.ToString());
        return documents.Select(d => d.AsEntity()).ToList();
    }
}
