using Mamey.Government.Modules.CMS.Core.Domain.Entities;
using Mamey.Government.Modules.CMS.Core.Domain.Repositories;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CMS.Core.Mongo.Documents;
using GovTenantId = Mamey.Types.TenantId;
using Mamey.MicroMonolith.Infrastructure.Mongo;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.CMS.Core.Mongo.Repositories;

internal class ContentMongoRepository : IContentRepository
{
    private readonly IMongoRepository<ContentDocument, Guid> _repository;
    private readonly ILogger<ContentMongoRepository> _logger;

    public ContentMongoRepository(
        IMongoRepository<ContentDocument, Guid> repository,
        ILogger<ContentMongoRepository> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Content?> GetAsync(ContentId id, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(id.Value);
        return document?.AsEntity();
    }

    public async Task AddAsync(Content content, CancellationToken cancellationToken = default)
    {
        var document = new ContentDocument(content);
        await _repository.AddAsync(document);
    }

    public async Task UpdateAsync(Content content, CancellationToken cancellationToken = default)
    {
        var document = new ContentDocument(content);
        await _repository.UpdateAsync(document);
    }

    public async Task DeleteAsync(ContentId id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id.Value);
    }

    public async Task<bool> ExistsAsync(ContentId id, CancellationToken cancellationToken = default)
    {
        return await _repository.ExistsAsync(d => d.Id == id.Value);
    }

    public async Task<IReadOnlyList<Content>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(_ => true);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<Content?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(d => d.Slug == slug);
        return document?.AsEntity();
    }

    public async Task<IReadOnlyList<Content>> GetByTenantAsync(GovTenantId tenantId, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.TenantId == tenantId.Value);
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Content>> GetByStatusAsync(ContentStatus status, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.Status == status.ToString());
        return documents.Select(d => d.AsEntity()).ToList();
    }

    public async Task<IReadOnlyList<Content>> GetByContentTypeAsync(string contentType, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.FindAsync(d => d.ContentType == contentType);
        return documents.Select(d => d.AsEntity()).ToList();
    }
}
