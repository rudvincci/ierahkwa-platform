using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.Logging;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Mongo.Repositories;

internal class UploadedDocumentMongoRepository : IUploadedDocumentRepository
{
    private readonly IMongoRepository<UploadedDocument, Guid> _repository;
    private readonly ILogger<UploadedDocumentMongoRepository> _logger;

    public UploadedDocumentMongoRepository(
        IMongoRepository<UploadedDocument, Guid> repository,
        ILogger<UploadedDocumentMongoRepository> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<UploadedDocument?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetAsync(id);
    }

    public async Task AddAsync(UploadedDocument document, CancellationToken cancellationToken = default)
    {
        await _repository.AddAsync(document);
    }

    public async Task UpdateAsync(UploadedDocument document, CancellationToken cancellationToken = default)
    {
        await _repository.UpdateAsync(document);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(d => d.Id == id);
    }

    public async Task<IReadOnlyList<UploadedDocument>> GetByApplicationIdAsync(AppId applicationId, CancellationToken cancellationToken = default)
    {
        return await _repository.FindAsync(d => d.ApplicationId == applicationId);
    }

    public async Task<IReadOnlyList<UploadedDocument>> GetByDocumentTypeAsync(string documentType, CancellationToken cancellationToken = default)
    {
        return await _repository.FindAsync(d => d.DocumentType == documentType);
    }

    public async Task<IReadOnlyList<UploadedDocument>> GetByApplicationIdAndTypeAsync(AppId applicationId, string documentType, CancellationToken cancellationToken = default)
    {
        return await _repository.FindAsync(d => d.ApplicationId == applicationId && d.DocumentType == documentType);
    }
}
