using Mamey.Government.Modules.Documents.Core.Domain.Entities;
using Mamey.Government.Modules.Documents.Core.Domain.Repositories;
using Mamey.Government.Modules.Documents.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Documents.Core.Mongo.Documents;
using Mamey.Types;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Documents.Core.Mongo.Repositories;

internal class DocumentMongoRepository : IDocumentRepository
{
    private readonly IMongoRepository<DocumentDocument, Guid> _repository;
    private readonly ILogger<DocumentMongoRepository> _logger;

    public DocumentMongoRepository(
        IMongoRepository<DocumentDocument, Guid> repository,
        ILogger<DocumentMongoRepository> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Document?> GetAsync(DocumentId id, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(id.Value);
        return document?.AsEntity();
    }

    public async Task AddAsync(Document document, CancellationToken cancellationToken = default)
    {
        var doc = new DocumentDocument(document);
        await _repository.AddAsync(doc);
    }

    public async Task UpdateAsync(Document document, CancellationToken cancellationToken = default)
    {
        var doc = new DocumentDocument(document);
        await _repository.UpdateAsync(doc);
    }

    public async Task DeleteAsync(DocumentId id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id.Value);
    }

    public async Task<bool> ExistsAsync(DocumentId id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(d => d.Id == id.Value);
    }

    public async Task<IReadOnlyList<Document>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(_ => true);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Document>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.TenantId == tenantId.Value);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Document>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.Category == category);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Document>> GetByStorageKeyAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.StorageKey == storageKey);
        return documents.Select(d => d.AsEntity()).ToList();
    }
}
