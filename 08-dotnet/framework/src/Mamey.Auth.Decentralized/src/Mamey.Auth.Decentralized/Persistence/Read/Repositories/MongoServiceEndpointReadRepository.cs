using MongoDB.Driver;
using Mamey.Auth.Decentralized.Persistence.Read.Models;

namespace Mamey.Auth.Decentralized.Persistence.Read.Repositories;

/// <summary>
/// MongoDB implementation of Service Endpoint read repository
/// </summary>
public class MongoServiceEndpointReadRepository : IServiceEndpointReadRepository
{
    private readonly IMongoCollection<ServiceEndpointReadModel> _collection;
    private readonly ILogger<MongoServiceEndpointReadRepository> _logger;

    public MongoServiceEndpointReadRepository(IMongoCollection<ServiceEndpointReadModel> collection, ILogger<MongoServiceEndpointReadRepository> logger)
    {
        _collection = collection;
        _logger = logger;
    }

    public async Task<ServiceEndpointReadModel?> GetByIdAsync(string serviceEndpointId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting service endpoint by ID: {Id}", serviceEndpointId);
            var result = await _collection.Find(s => s.ServiceEndpointId == serviceEndpointId && s.IsActive).FirstOrDefaultAsync(cancellationToken);
            _logger.LogDebug("Found service endpoint: {Found}", result != null);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service endpoint by ID: {Id}", serviceEndpointId);
            throw;
        }
    }

    public async Task<IReadOnlyList<ServiceEndpointReadModel>> GetByDidAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting service endpoints by DID: {Did}", did);
            // This would need to be implemented based on how service endpoints are linked to DIDs
            // For now, we'll search by service endpoint ID pattern
            var results = await _collection.Find(s => s.ServiceEndpointId.StartsWith(did) && s.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} service endpoints for DID: {Did}", results.Count, did);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service endpoints by DID: {Did}", did);
            throw;
        }
    }

    public async Task<IReadOnlyList<ServiceEndpointReadModel>> GetByTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting service endpoints by type: {Type}", type);
            var results = await _collection.Find(s => s.Type == type && s.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} service endpoints for type: {Type}", results.Count, type);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service endpoints by type: {Type}", type);
            throw;
        }
    }

    public async Task<IReadOnlyList<ServiceEndpointReadModel>> GetByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting service endpoints by URL: {Url}", url);
            var results = await _collection.Find(s => s.ServiceEndpoint == url && s.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} service endpoints for URL: {Url}", results.Count, url);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service endpoints by URL: {Url}", url);
            throw;
        }
    }

    public async Task<IReadOnlyList<ServiceEndpointReadModel>> GetByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting service endpoints by tag: {Tag}", tag);
            var results = await _collection.Find(s => s.Tags.Contains(tag) && s.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} service endpoints for tag: {Tag}", results.Count, tag);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service endpoints by tag: {Tag}", tag);
            throw;
        }
    }

    public async Task<IReadOnlyList<ServiceEndpointReadModel>> GetByPriorityAsync(int priority, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting service endpoints by priority: {Priority}", priority);
            var results = await _collection.Find(s => s.Priority == priority && s.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} service endpoints for priority: {Priority}", results.Count, priority);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting service endpoints by priority: {Priority}", priority);
            throw;
        }
    }

    public async Task<IReadOnlyList<ServiceEndpointReadModel>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all active service endpoints");
            var results = await _collection.Find(s => s.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} active service endpoints", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active service endpoints");
            throw;
        }
    }

    public async Task<IReadOnlyList<ServiceEndpointReadModel>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Searching service endpoints with query: {Query}", query);
            
            var filter = Builders<ServiceEndpointReadModel>.Filter.And(
                Builders<ServiceEndpointReadModel>.Filter.Eq(s => s.IsActive, true),
                Builders<ServiceEndpointReadModel>.Filter.Or(
                    Builders<ServiceEndpointReadModel>.Filter.Regex(s => s.ServiceEndpointId, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<ServiceEndpointReadModel>.Filter.Regex(s => s.Type, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<ServiceEndpointReadModel>.Filter.Regex(s => s.ServiceEndpoint, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<ServiceEndpointReadModel>.Filter.Regex(s => s.Description, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<ServiceEndpointReadModel>.Filter.AnyIn(s => s.Tags, new[] { query })
                )
            );

            var results = await _collection.Find(filter).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} service endpoints matching query: {Query}", results.Count, query);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching service endpoints with query: {Query}", query);
            throw;
        }
    }

    public async Task<PaginatedResult<ServiceEndpointReadModel>> GetPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting paginated service endpoints - Page: {Page}, PageSize: {PageSize}", page, pageSize);
            
            var filter = Builders<ServiceEndpointReadModel>.Filter.Eq(s => s.IsActive, true);
            var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            
            var skip = (page - 1) * pageSize;
            var results = await _collection
                .Find(filter)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            var result = new PaginatedResult<ServiceEndpointReadModel>
            {
                Items = results,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            _logger.LogDebug("Retrieved {Count} service endpoints for page {Page} of {TotalPages}", 
                results.Count, page, result.TotalPages);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated service endpoints - Page: {Page}, PageSize: {PageSize}", page, pageSize);
            throw;
        }
    }

    public async Task<long> CountByTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Counting service endpoints by type: {Type}", type);
            var count = await _collection.CountDocumentsAsync(s => s.Type == type && s.IsActive, cancellationToken: cancellationToken);
            _logger.LogDebug("Found {Count} service endpoints for type: {Type}", count, type);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting service endpoints by type: {Type}", type);
            throw;
        }
    }

    public async Task<long> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Counting active service endpoints");
            var count = await _collection.CountDocumentsAsync(s => s.IsActive, cancellationToken: cancellationToken);
            _logger.LogDebug("Found {Count} active service endpoints", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting active service endpoints");
            throw;
        }
    }
}
