using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.Domain.Entities;
using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.Mongo.Documents;
using Mamey.Types;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Passports.Core.Mongo.Repositories;

internal class PassportMongoRepository : IPassportRepository
{
    private readonly IMongoRepository<PassportDocument, Guid> _repository;
    private readonly ILogger<PassportMongoRepository> _logger;

    public PassportMongoRepository(
        IMongoRepository<PassportDocument, Guid> repository,
        ILogger<PassportMongoRepository> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Passport?> GetAsync(PassportId id, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(id.Value);
        return document?.AsEntity();
    }

    public async Task AddAsync(Passport passport, CancellationToken cancellationToken = default)
    {
        var document = new PassportDocument(passport);
        await _repository.AddAsync(document);
    }

    public async Task UpdateAsync(Passport passport, CancellationToken cancellationToken = default)
    {
        var document = new PassportDocument(passport);
        await _repository.UpdateAsync(document);
    }

    public async Task DeleteAsync(PassportId id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id.Value);
    }

    public async Task<bool> ExistsAsync(PassportId id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(d => d.Id == id.Value);
    }

    public async Task<IReadOnlyList<Passport>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(_ => true);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<Passport?> GetByPassportNumberAsync(PassportNumber passportNumber, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(d => d.PassportNumber == passportNumber.Value);
        return document?.AsEntity();
    }

    public async Task<IReadOnlyList<Passport>> GetByCitizenAsync(CitizenId citizenId, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.CitizenId == citizenId.Value);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Passport>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.TenantId == tenantId.Value);
        return documents.Select(d => d.AsEntity()).ToList();
    }
}
