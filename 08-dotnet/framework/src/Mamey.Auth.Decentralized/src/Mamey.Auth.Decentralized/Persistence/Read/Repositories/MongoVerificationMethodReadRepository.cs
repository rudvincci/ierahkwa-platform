using MongoDB.Driver;
using Mamey.Auth.Decentralized.Persistence.Read.Models;

namespace Mamey.Auth.Decentralized.Persistence.Read.Repositories;

/// <summary>
/// MongoDB implementation of Verification Method read repository
/// </summary>
public class MongoVerificationMethodReadRepository : IVerificationMethodReadRepository
{
    private readonly IMongoCollection<VerificationMethodReadModel> _collection;
    private readonly ILogger<MongoVerificationMethodReadRepository> _logger;

    public MongoVerificationMethodReadRepository(IMongoCollection<VerificationMethodReadModel> collection, ILogger<MongoVerificationMethodReadRepository> logger)
    {
        _collection = collection;
        _logger = logger;
    }

    public async Task<VerificationMethodReadModel?> GetByIdAsync(string verificationMethodId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting verification method by ID: {Id}", verificationMethodId);
            var result = await _collection.Find(v => v.VerificationMethodId == verificationMethodId && v.IsActive).FirstOrDefaultAsync(cancellationToken);
            _logger.LogDebug("Found verification method: {Found}", result != null);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting verification method by ID: {Id}", verificationMethodId);
            throw;
        }
    }

    public async Task<IReadOnlyList<VerificationMethodReadModel>> GetByDidAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting verification methods by DID: {Did}", did);
            // This would need to be implemented based on how verification methods are linked to DIDs
            // For now, we'll search by verification method ID pattern
            var results = await _collection.Find(v => v.VerificationMethodId.StartsWith(did) && v.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} verification methods for DID: {Did}", results.Count, did);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting verification methods by DID: {Did}", did);
            throw;
        }
    }

    public async Task<IReadOnlyList<VerificationMethodReadModel>> GetByTypeAsync(string type, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting verification methods by type: {Type}", type);
            var results = await _collection.Find(v => v.Type == type && v.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} verification methods for type: {Type}", results.Count, type);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting verification methods by type: {Type}", type);
            throw;
        }
    }

    public async Task<IReadOnlyList<VerificationMethodReadModel>> GetByAlgorithmAsync(string algorithm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting verification methods by algorithm: {Algorithm}", algorithm);
            var results = await _collection.Find(v => v.Algorithm == algorithm && v.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} verification methods for algorithm: {Algorithm}", results.Count, algorithm);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting verification methods by algorithm: {Algorithm}", algorithm);
            throw;
        }
    }

    public async Task<IReadOnlyList<VerificationMethodReadModel>> GetByCurveAsync(string curve, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting verification methods by curve: {Curve}", curve);
            var results = await _collection.Find(v => v.Curve == curve && v.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} verification methods for curve: {Curve}", results.Count, curve);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting verification methods by curve: {Curve}", curve);
            throw;
        }
    }

    public async Task<IReadOnlyList<VerificationMethodReadModel>> GetByControllerAsync(string controller, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting verification methods by controller: {Controller}", controller);
            var results = await _collection.Find(v => v.Controller == controller && v.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} verification methods for controller: {Controller}", results.Count, controller);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting verification methods by controller: {Controller}", controller);
            throw;
        }
    }

    public async Task<IReadOnlyList<VerificationMethodReadModel>> GetByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting verification methods by tag: {Tag}", tag);
            var results = await _collection.Find(v => v.Tags.Contains(tag) && v.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} verification methods for tag: {Tag}", results.Count, tag);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting verification methods by tag: {Tag}", tag);
            throw;
        }
    }

    public async Task<IReadOnlyList<VerificationMethodReadModel>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all active verification methods");
            var results = await _collection.Find(v => v.IsActive).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} active verification methods", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active verification methods");
            throw;
        }
    }

    public async Task<IReadOnlyList<VerificationMethodReadModel>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Searching verification methods with query: {Query}", query);
            
            var filter = Builders<VerificationMethodReadModel>.Filter.And(
                Builders<VerificationMethodReadModel>.Filter.Eq(v => v.IsActive, true),
                Builders<VerificationMethodReadModel>.Filter.Or(
                    Builders<VerificationMethodReadModel>.Filter.Regex(v => v.VerificationMethodId, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<VerificationMethodReadModel>.Filter.Regex(v => v.Type, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<VerificationMethodReadModel>.Filter.Regex(v => v.Algorithm, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<VerificationMethodReadModel>.Filter.Regex(v => v.Curve, new MongoDB.Bson.BsonRegularExpression(query, "i")),
                    Builders<VerificationMethodReadModel>.Filter.AnyIn(v => v.Tags, new[] { query })
                )
            );

            var results = await _collection.Find(filter).ToListAsync(cancellationToken);
            _logger.LogDebug("Found {Count} verification methods matching query: {Query}", results.Count, query);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching verification methods with query: {Query}", query);
            throw;
        }
    }

    public async Task<PaginatedResult<VerificationMethodReadModel>> GetPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting paginated verification methods - Page: {Page}, PageSize: {PageSize}", page, pageSize);
            
            var filter = Builders<VerificationMethodReadModel>.Filter.Eq(v => v.IsActive, true);
            var totalCount = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
            
            var skip = (page - 1) * pageSize;
            var results = await _collection
                .Find(filter)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            var result = new PaginatedResult<VerificationMethodReadModel>
            {
                Items = results,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            _logger.LogDebug("Retrieved {Count} verification methods for page {Page} of {TotalPages}", 
                results.Count, page, result.TotalPages);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated verification methods - Page: {Page}, PageSize: {PageSize}", page, pageSize);
            throw;
        }
    }

    public async Task<long> CountByAlgorithmAsync(string algorithm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Counting verification methods by algorithm: {Algorithm}", algorithm);
            var count = await _collection.CountDocumentsAsync(v => v.Algorithm == algorithm && v.IsActive, cancellationToken: cancellationToken);
            _logger.LogDebug("Found {Count} verification methods for algorithm: {Algorithm}", count, algorithm);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting verification methods by algorithm: {Algorithm}", algorithm);
            throw;
        }
    }

    public async Task<long> CountActiveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Counting active verification methods");
            var count = await _collection.CountDocumentsAsync(v => v.IsActive, cancellationToken: cancellationToken);
            _logger.LogDebug("Found {Count} active verification methods", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error counting active verification methods");
            throw;
        }
    }
}
