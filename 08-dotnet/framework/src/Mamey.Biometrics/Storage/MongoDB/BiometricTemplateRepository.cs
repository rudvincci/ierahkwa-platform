using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Mamey.Biometrics.Storage.MongoDB.Models;

namespace Mamey.Biometrics.Storage.MongoDB;

/// <summary>
/// MongoDB repository for biometric template storage.
/// </summary>
public class BiometricTemplateRepository : IBiometricTemplateRepository
{
    private readonly IMongoCollection<BiometricTemplateDocument> _collection;
    private readonly ILogger<BiometricTemplateRepository> _logger;
    private readonly MongoDBOptions _options;

    /// <summary>
    /// Initializes a new instance of the BiometricTemplateRepository.
    /// </summary>
    public BiometricTemplateRepository(
        IMongoDatabase database,
        IOptions<MongoDBOptions> options,
        ILogger<BiometricTemplateRepository> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _collection = database.GetCollection<BiometricTemplateDocument>(_options.TemplatesCollectionName);
        
        // Create indexes
        CreateIndexes();
    }

    /// <inheritdoc/>
    public async Task<BiometricTemplateDocument> AddAsync(BiometricTemplateDocument template, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Adding biometric template {TemplateId} for subject {SubjectId}", 
                template.TemplateId, template.SubjectId);

            template.CreatedAt = DateTime.UtcNow;
            template.UpdatedAt = DateTime.UtcNow;

            await _collection.InsertOneAsync(template, cancellationToken: cancellationToken);
            
            _logger.LogInformation("Biometric template {TemplateId} added successfully", template.TemplateId);
            return template;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding biometric template {TemplateId}", template.TemplateId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<BiometricTemplateDocument?> GetByIdAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting biometric template {TemplateId}", templateId);

            var filter = Builders<BiometricTemplateDocument>.Filter.Eq(t => t.TemplateId, templateId);
            var template = await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
            
            if (template == null)
            {
                _logger.LogDebug("Biometric template {TemplateId} not found", templateId);
            }

            return template;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting biometric template {TemplateId}", templateId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<List<BiometricTemplateDocument>> GetBySubjectIdAsync(Guid subjectId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting biometric templates for subject {SubjectId}", subjectId);

            var filter = Builders<BiometricTemplateDocument>.Filter.Eq(t => t.SubjectId, subjectId);
            var templates = await _collection.Find(filter)
                .SortByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
            
            _logger.LogDebug("Found {Count} templates for subject {SubjectId}", templates.Count, subjectId);
            return templates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting biometric templates for subject {SubjectId}", subjectId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<PagedResult<BiometricTemplateDocument>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting paged templates: page {Page}, size {PageSize}", page, pageSize);

            var skip = (page - 1) * pageSize;
            
            var totalCount = await _collection.CountDocumentsAsync(FilterDefinition<BiometricTemplateDocument>.Empty, cancellationToken: cancellationToken);
            
            var templates = await _collection.Find(FilterDefinition<BiometricTemplateDocument>.Empty)
                .SortByDescending(t => t.CreatedAt)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            var result = new PagedResult<BiometricTemplateDocument>
            {
                Items = templates,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            _logger.LogDebug("Retrieved {Count} templates (page {Page} of {TotalPages})", 
                templates.Count, page, result.TotalPages);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged templates");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<List<BiometricTemplateDocument>> SearchAsync(
        Guid? subjectId = null,
        string[]? tags = null,
        double? minQualityScore = null,
        DateTime? createdAfter = null,
        DateTime? createdBefore = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Searching templates with filters: subjectId={SubjectId}, tags={Tags}, minQuality={MinQuality}", 
                subjectId, tags?.Length, minQualityScore);

            var filters = new List<FilterDefinition<BiometricTemplateDocument>>();

            if (subjectId.HasValue)
            {
                filters.Add(Builders<BiometricTemplateDocument>.Filter.Eq(t => t.SubjectId, subjectId.Value));
            }

            if (tags != null && tags.Length > 0)
            {
                filters.Add(Builders<BiometricTemplateDocument>.Filter.All(t => t.Tags, tags));
            }

            if (minQualityScore.HasValue)
            {
                filters.Add(Builders<BiometricTemplateDocument>.Filter.Gte(t => t.Metadata.QualityScore, minQualityScore.Value));
            }

            if (createdAfter.HasValue)
            {
                filters.Add(Builders<BiometricTemplateDocument>.Filter.Gte(t => t.CreatedAt, createdAfter.Value));
            }

            if (createdBefore.HasValue)
            {
                filters.Add(Builders<BiometricTemplateDocument>.Filter.Lte(t => t.CreatedAt, createdBefore.Value));
            }

            var filter = filters.Count > 0 
                ? Builders<BiometricTemplateDocument>.Filter.And(filters)
                : FilterDefinition<BiometricTemplateDocument>.Empty;

            var templates = await _collection.Find(filter)
                .SortByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Search returned {Count} templates", templates.Count);
            return templates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching templates");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<BiometricTemplateDocument?> UpdateAsync(BiometricTemplateDocument template, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Updating biometric template {TemplateId}", template.TemplateId);

            template.UpdatedAt = DateTime.UtcNow;

            var filter = Builders<BiometricTemplateDocument>.Filter.Eq(t => t.TemplateId, template.TemplateId);
            var result = await _collection.ReplaceOneAsync(filter, template, cancellationToken: cancellationToken);
            
            if (result.MatchedCount == 0)
            {
                _logger.LogWarning("Biometric template {TemplateId} not found for update", template.TemplateId);
                return null;
            }

            _logger.LogInformation("Biometric template {TemplateId} updated successfully", template.TemplateId);
            return template;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating biometric template {TemplateId}", template.TemplateId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Deleting biometric template {TemplateId}", templateId);

            var filter = Builders<BiometricTemplateDocument>.Filter.Eq(t => t.TemplateId, templateId);
            var result = await _collection.DeleteOneAsync(filter, cancellationToken);
            
            var deleted = result.DeletedCount > 0;
            
            if (deleted)
            {
                _logger.LogInformation("Biometric template {TemplateId} deleted successfully", templateId);
            }
            else
            {
                _logger.LogWarning("Biometric template {TemplateId} not found for deletion", templateId);
            }

            return deleted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting biometric template {TemplateId}", templateId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<List<BiometricTemplateDocument>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting all biometric templates");

            var templates = await _collection.Find(FilterDefinition<BiometricTemplateDocument>.Empty)
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Retrieved {Count} templates", templates.Count);
            return templates;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all templates");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting template count");

            var count = await _collection.CountDocumentsAsync(FilterDefinition<BiometricTemplateDocument>.Empty, cancellationToken: cancellationToken);
            
            _logger.LogDebug("Template count: {Count}", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting template count");
            throw;
        }
    }

    /// <summary>
    /// Create MongoDB indexes for optimal query performance.
    /// </summary>
    private void CreateIndexes()
    {
        try
        {
            _logger.LogDebug("Creating MongoDB indexes");

            // Index on template_id for fast lookups
            var templateIdIndex = Builders<BiometricTemplateDocument>.IndexKeys.Ascending(t => t.TemplateId);
            _collection.Indexes.CreateOne(new CreateIndexModel<BiometricTemplateDocument>(templateIdIndex, new CreateIndexOptions { Unique = true }));

            // Index on subject_id for fast subject queries
            var subjectIdIndex = Builders<BiometricTemplateDocument>.IndexKeys.Ascending(t => t.SubjectId);
            _collection.Indexes.CreateOne(new CreateIndexModel<BiometricTemplateDocument>(subjectIdIndex));

            // Index on created_at for sorting
            var createdAtIndex = Builders<BiometricTemplateDocument>.IndexKeys.Descending(t => t.CreatedAt);
            _collection.Indexes.CreateOne(new CreateIndexModel<BiometricTemplateDocument>(createdAtIndex));

            // Index on tags for tag-based searches
            var tagsIndex = Builders<BiometricTemplateDocument>.IndexKeys.Ascending(t => t.Tags);
            _collection.Indexes.CreateOne(new CreateIndexModel<BiometricTemplateDocument>(tagsIndex));

            // Index on quality score for quality-based searches
            var qualityScoreIndex = Builders<BiometricTemplateDocument>.IndexKeys.Ascending(t => t.Metadata.QualityScore);
            _collection.Indexes.CreateOne(new CreateIndexModel<BiometricTemplateDocument>(qualityScoreIndex));

            // Compound index for common queries (subject_id + created_at)
            var compoundIndex = Builders<BiometricTemplateDocument>.IndexKeys
                .Ascending(t => t.SubjectId)
                .Descending(t => t.CreatedAt);
            _collection.Indexes.CreateOne(new CreateIndexModel<BiometricTemplateDocument>(compoundIndex));

            _logger.LogInformation("MongoDB indexes created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating MongoDB indexes");
            // Don't throw here as the service can still work without indexes
        }
    }
}
