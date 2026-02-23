using Mamey.Government.Modules.CMS.Core.Domain.Entities;
using Mamey.Government.Modules.CMS.Core.Domain.Repositories;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;
using GovTenantId = Mamey.Types.TenantId;
using Mamey.Persistence.Redis;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.CMS.Core.Redis.Repositories;

internal class ContentRedisRepository : IContentRepository
{
    private readonly ICache _cache;
    private readonly ILogger<ContentRedisRepository> _logger;
    private const string ContentPrefix = "cms:contents:";

    public ContentRedisRepository(
        ICache cache,
        ILogger<ContentRedisRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<Content?> GetAsync(ContentId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.GetAsync<Content>($"{ContentPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get content from Redis: {ContentId}", id.Value);
            return null;
        }
    }

    public async Task AddAsync(Content content, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.SetAsync($"{ContentPrefix}{content.Id.Value}", content, TimeSpan.FromHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add content to Redis: {ContentId}", content.Id.Value);
        }
    }

    public async Task UpdateAsync(Content content, CancellationToken cancellationToken = default)
    {
        await AddAsync(content, cancellationToken);
    }

    public async Task DeleteAsync(ContentId id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.DeleteAsync<Content>($"{ContentPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete content from Redis: {ContentId}", id.Value);
        }
    }

    public async Task<bool> ExistsAsync(ContentId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.KeyExistsAsync($"{ContentPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check content existence in Redis: {ContentId}", id.Value);
            return false;
        }
    }

    public Task<IReadOnlyList<Content>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Content>>(new List<Content>());
    }

    public Task<Content?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Content?>(null);
    }

    public Task<IReadOnlyList<Content>> GetByTenantAsync(GovTenantId tenantId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Content>>(new List<Content>());
    }

    public Task<IReadOnlyList<Content>> GetByStatusAsync(ContentStatus status, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Content>>(new List<Content>());
    }

    public Task<IReadOnlyList<Content>> GetByContentTypeAsync(string contentType, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Content>>(new List<Content>());
    }
}
