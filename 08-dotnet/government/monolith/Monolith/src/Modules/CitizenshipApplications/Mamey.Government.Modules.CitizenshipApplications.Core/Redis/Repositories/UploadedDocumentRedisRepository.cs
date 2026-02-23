using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Persistence.Redis;
using Microsoft.Extensions.Logging;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Redis.Repositories;

internal class UploadedDocumentRedisRepository : IUploadedDocumentRepository
{
    private readonly ICache _cache;
    private readonly ILogger<UploadedDocumentRedisRepository> _logger;
    private const string DocumentPrefix = "citizenship-applications:documents:";
    private const string ApplicationDocumentsPrefix = "citizenship-applications:documents:application:";

    public UploadedDocumentRedisRepository(
        ICache cache,
        ILogger<UploadedDocumentRedisRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<UploadedDocument?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.GetAsync<UploadedDocument>($"{DocumentPrefix}{id}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get document from Redis: {DocumentId}", id);
            return null;
        }
    }

    public async Task AddAsync(UploadedDocument document, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.SetAsync($"{DocumentPrefix}{document.Id}", document, TimeSpan.FromHours(1));
            
            // Also cache by application ID for lookup
            var applicationKey = $"{ApplicationDocumentsPrefix}{document.ApplicationId.Value}";
            var documents = await _cache.GetAsync<List<Guid>>(applicationKey) ?? new List<Guid>();
            if (!documents.Contains(document.Id))
            {
                documents.Add(document.Id);
                await _cache.SetAsync(applicationKey, documents, TimeSpan.FromHours(1));
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add document to Redis: {DocumentId}", document.Id);
        }
    }

    public async Task UpdateAsync(UploadedDocument document, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.SetAsync($"{DocumentPrefix}{document.Id}", document, TimeSpan.FromHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update document in Redis: {DocumentId}", document.Id);
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var document = await GetAsync(id, cancellationToken);
            if (document != null)
            {
                await _cache.DeleteAsync<UploadedDocument>($"{DocumentPrefix}{id}");
                
                // Remove from application index
                var applicationKey = $"{ApplicationDocumentsPrefix}{document.ApplicationId.Value}";
                var documents = await _cache.GetAsync<List<Guid>>(applicationKey);
                if (documents != null)
                {
                    documents.Remove(id);
                    await _cache.SetAsync(applicationKey, documents, TimeSpan.FromHours(1));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete document from Redis: {DocumentId}", id);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var document = await GetAsync(id, cancellationToken);
            return document != null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check document existence in Redis: {DocumentId}", id);
            return false;
        }
    }

    public async Task<IReadOnlyList<UploadedDocument>> GetByApplicationIdAsync(AppId applicationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var applicationKey = $"{ApplicationDocumentsPrefix}{applicationId.Value}";
            var documentIds = await _cache.GetAsync<List<Guid>>(applicationKey);
            
            if (documentIds == null || !documentIds.Any())
            {
                return Array.Empty<UploadedDocument>();
            }

            var documents = new List<UploadedDocument>();
            foreach (var id in documentIds)
            {
                var document = await GetAsync(id, cancellationToken);
                if (document != null)
                {
                    documents.Add(document);
                }
            }
            
            return documents;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get documents by application ID from Redis: {ApplicationId}", applicationId.Value);
            return Array.Empty<UploadedDocument>();
        }
    }

    public async Task<IReadOnlyList<UploadedDocument>> GetByDocumentTypeAsync(string documentType, CancellationToken cancellationToken = default)
    {
        // Redis doesn't support querying by property, so we return empty
        // This would need to be implemented with a separate index or use a different approach
        _logger.LogWarning("GetByDocumentTypeAsync is not efficiently supported in Redis cache");
        return Array.Empty<UploadedDocument>();
    }

    public async Task<IReadOnlyList<UploadedDocument>> GetByApplicationIdAndTypeAsync(AppId applicationId, string documentType, CancellationToken cancellationToken = default)
    {
        try
        {
            var documents = await GetByApplicationIdAsync(applicationId, cancellationToken);
            return documents.Where(d => d.DocumentType == documentType).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get documents by application ID and type from Redis: {ApplicationId}, {DocumentType}", applicationId.Value, documentType);
            return Array.Empty<UploadedDocument>();
        }
    }
}
