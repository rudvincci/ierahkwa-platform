using MongoDB.Driver;
using Mamey.Auth.Decentralized.Persistence.Read.Models;

namespace Mamey.Auth.Decentralized.Persistence.Read.Repositories;

/// <summary>
/// MongoDB implementation of DID Document read repository
/// </summary>
public class MongoDidDocumentReadRepository : IDidDocumentReadRepository
{
    private readonly IMongoCollection<DidDocumentReadModel> _collection;
    private readonly ILogger<MongoDidDocumentReadRepository> _logger;

    public MongoDidDocumentReadRepository(IMongoCollection<DidDocumentReadModel> collection, ILogger<MongoDidDocumentReadRepository> logger)
    {
        _collection = collection;
        _logger = logger;
    }

    public async Task<DidDocumentReadModel?> GetByDidAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting DID Document by DID: {Did}", did);
            var result = await _collection.Find(d => d.Did == did && d.IsActive).FirstOrDefaultAsync(cancellationToken);
            _logger.LogDebug("Found DID Document: {Found}", result != null);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DID Document by DID: {Did}", did);
            throw;
        }
    }

    public async Task<DidDocumentReadModel?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting DID Document by ID: {Id}", id);
            var result = await _collection.Find(d => d.Id == id && d.IsActive).FirstOrDefaultAsync(cancellationToken);
            _logger.LogDebug("Found DID Document: {Found}", result != null);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DID Document by ID: {Id}", id);
            throw;
        }
    }

    public async Task<IReadOnlyList<DidDocumentReadModel>> GetByMethodAsync(string method, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting DID Documents by method: {Method}", method);
            var results = await _collection.Find(d => d.Method == method && d.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} DID Documents for method: {Method}", results.Count, method);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DID Documents by method: {Method}", method);
            throw;
        }
    }

    public async Task<IReadOnlyList<DidDocumentReadModel>> GetByControllerAsync(string controller, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting DID Documents by controller: {Controller}", controller);
            var results = await _collection.Find(d => d.Controller == controller && d.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} DID Documents for controller: {Controller}", results.Count, controller);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DID Documents by controller: {Controller}", controller);
            throw;
        }
    }

    public async Task<IReadOnlyList<DidDocumentReadModel>> GetByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting DID Documents by tag: {Tag}", tag);
            var results = await _collection.Find(d => d.Tags.Contains(tag) && d.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} DID Documents for tag: {Tag}", results.Count, tag);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DID Documents by tag: {Tag}", tag);
            throw;
        }
    }

    public async Task<IReadOnlyList<DidDocumentReadModel>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all active DID Documents");
            var results = await _collection.Find(d => d.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} active DID Documents", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active DID Documents");
            throw;
        }
    }

    public async Task<IReadOnlyList<DidDocumentReadModel>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Searching DID Documents with query: {Query}", query);
            
            var filter = Builders<DidDocumentReadModel>.Filter.And(
                Builders<DidDocumentReadModel>.Filter.Eq(d => d.IsActive, true),
                Builders<DidDocumentReadModel>.Filter.Or(
                    Builders<DidDocumentReadModel>.Filter.Regex(d => d.Did, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<DidDocumentReadModel>.Filter.Regex(d => d.Method, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<DidDocumentReadModel>.Filter.Regex(d => d.Controller, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<DidDocumentReadModel>.Filter.AnyIn(d => d.Tags, new[] { query })
                )
            );

            var results = await _collection.Find(filter).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} DID Documents matching query: {Query}", results.Count, query);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching DID Documents with query: {Query}", query);
            throw;
        }
    }

    public async Task<PaginatedResult<DidDocumentReadModel>> GetPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting paginated DID Documents - Page: {Page}, PageSize: {PageSize}", page, pageSize);
            
            var filter = Builders<DidDocumentReadModel>.Filter.Eq(d => d.IsActive, true);
            var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            
            var skip = (page - 1) * pageSize;
            var results = await _collection
                .Find(filter)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            var result = new PaginatedResult<DidDocumentReadModel>
            {
                Items = results,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            _logger.LogDebug("Retrieved {Count} DID Documents for page {Page} of {TotalPages}", 
                results.Count, page, result.TotalPages);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated DID Documents - Page: {Page}, PageSize: {PageSize}", page, pageSize);
            throw;
        }
    }

    public async Task<IReadOnlyList<DidDocumentReadModel>> GetCreatedAfterAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting DID Documents created after: {Date}", date);
            var results = await _collection.Find(d => d.CreatedAt > date && d.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} DID Documents created after: {Date}", results.Count, date);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DID Documents created after: {Date}", date);
            throw;
        }
    }

    public async Task<IReadOnlyList<DidDocumentReadModel>> GetUpdatedAfterAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting DID Documents updated after: {Date}", date);
            var results = await _collection.Find(d => d.UpdatedAt > date && d.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} DID Documents updated after: {Date}", results.Count, date);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting DID Documents updated after: {Date}", date);
            throw;
        }
    }

    public async Task<long> CountByMethodAsync(string method, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Counting DID Documents by method: {Method}", method);
            var count = await _collection.CountDocumentsAsync(d => d.Method == method && d.IsActive, cancellationToken: cancellationToken);
            _logger.LogDebug("Found {Count} DID Documents for method: {Method}", count, method);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting DID Documents by method: {Method}", method);
            throw;
        }
    }

    public async Task<long> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Counting active DID Documents");
            var count = await _collection.CountDocumentsAsync(d => d.IsActive, cancellationToken: cancellationToken);
            _logger.LogDebug("Found {Count} active DID Documents", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting active DID Documents");
            throw;
        }
    }
}
