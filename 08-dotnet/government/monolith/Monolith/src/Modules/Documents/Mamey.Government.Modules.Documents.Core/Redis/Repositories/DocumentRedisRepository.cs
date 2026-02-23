using Mamey.Government.Modules.Documents.Core.Domain.Entities;
using Mamey.Government.Modules.Documents.Core.Domain.Repositories;
using Mamey.Government.Modules.Documents.Core.Domain.ValueObjects;
using Mamey.Types;
using Mamey.Persistence.Redis;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Documents.Core.Redis.Repositories;

internal class DocumentRedisRepository : IDocumentRepository
{
    private readonly ICache _cache;
    private readonly ILogger<DocumentRedisRepository> _logger;
    private const string DocumentPrefix = "documents:documents:";

    public DocumentRedisRepository(
        ICache cache,
        ILogger<DocumentRedisRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<Document?> GetAsync(DocumentId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.GetAsync<Document>($"{DocumentPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get document from Redis: {DocumentId}", id.Value);
            return null;
        }
    }

    public async Task AddAsync(Document document, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.SetAsync($"{DocumentPrefix}{document.Id.Value}", document, TimeSpan.FromHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add document to Redis: {DocumentId}", document.Id.Value);
        }
    }

    public async Task UpdateAsync(Document document, CancellationToken cancellationToken = default)
    {
        await AddAsync(document, cancellationToken);
    }

    public async Task DeleteAsync(DocumentId id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.DeleteAsync<Document>($"{DocumentPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete document from Redis: {DocumentId}", id.Value);
        }
    }

    public async Task<bool> ExistsAsync(DocumentId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.KeyExistsAsync($"{DocumentPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check document existence in Redis: {DocumentId}", id.Value);
            return false;
        }
    }

    public Task<IReadOnlyList<Document>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Document>>(new List<Document>());
    }

    public Task<IReadOnlyList<Document>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Document>>(new List<Document>());
    }

    public Task<IReadOnlyList<Document>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Document>>(new List<Document>());
    }

    public Task<IReadOnlyList<Document>> GetByStorageKeyAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Document>>(new List<Document>());
    }
}
