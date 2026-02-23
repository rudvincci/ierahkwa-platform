using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CitizenshipApplications.Core.Mongo.Documents;
using Mamey.Types;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.Logging;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Mongo.Repositories;

internal class ApplicationMongoRepository : IApplicationRepository
{
    private readonly IMongoRepository<ApplicationDocument, Guid> _repository;
    private readonly ILogger<ApplicationMongoRepository> _logger;

    public ApplicationMongoRepository(
        IMongoRepository<ApplicationDocument, Guid> repository,
        ILogger<ApplicationMongoRepository> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CitizenshipApplication?> GetAsync(AppId id, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(id.Value);
        return document?.AsEntity();
    }

    public async Task AddAsync(CitizenshipApplication application, CancellationToken cancellationToken = default)
    {
        var document = new ApplicationDocument(application);
        await _repository.AddAsync(document);
    }

    public async Task UpdateAsync(CitizenshipApplication application, CancellationToken cancellationToken = default)
    {
        var document = new ApplicationDocument(application);
        await _repository.UpdateAsync(document);
    }

    public async Task DeleteAsync(AppId id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id.Value);
    }

    public async Task<bool> ExistsAsync(AppId id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(d => d.Id == id.Value);
    }

    public async Task<IReadOnlyList<CitizenshipApplication>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(_ => true);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<IList<CitizenshipApplication>> GetAllByApplicationEmail(string email, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.Email == email);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<CitizenshipApplication?> GetByApplicationEmail(string email, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(d => d.Email == email);
        return document?.AsEntity();
    }

    public async Task<CitizenshipApplication?> GetByApplicationNumberAsync(ApplicationNumber applicationNumber, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(d => d.ApplicationNumber == applicationNumber.Value);
        return document?.AsEntity();
    }

    public async Task<IReadOnlyList<CitizenshipApplication>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.TenantId == tenantId.Value);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<CitizenshipApplication>> GetByStatusAsync(ApplicationStatus status, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.Status == status.ToString());
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<int> GetCountByYearAsync(TenantId tenantId, int year, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.TenantId == tenantId.Value && d.CreatedAt.Year == year);
        return documents.Count();
    }
}
