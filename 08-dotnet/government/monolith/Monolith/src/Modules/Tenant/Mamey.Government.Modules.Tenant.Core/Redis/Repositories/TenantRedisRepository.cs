using Mamey.Government.Modules.Tenant.Core.Domain.Entities;
using Mamey.Government.Modules.Tenant.Core.Domain.Repositories;
using Mamey.Persistence.Redis;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Tenant.Core.Redis.Repositories;

internal class TenantRedisRepository : ITenantRepository
{
    private readonly ICache _cache;
    private readonly ILogger<TenantRedisRepository> _logger;
    private const string TenantPrefix = "tenant:tenants:";

    public TenantRedisRepository(
        ICache cache,
        ILogger<TenantRedisRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TenantEntity?> GetAsync(TenantId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.GetAsync<TenantEntity>($"{TenantPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get tenant from Redis: {TenantId}", id.Value);
            return null;
        }
    }

    public async Task AddAsync(TenantEntity tenant, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.SetAsync($"{TenantPrefix}{tenant.Id.Value}", tenant, TimeSpan.FromHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add tenant to Redis: {TenantId}", tenant.Id.Value);
        }
    }

    public async Task UpdateAsync(TenantEntity tenant, CancellationToken cancellationToken = default)
    {
        await AddAsync(tenant, cancellationToken);
    }

    public async Task DeleteAsync(TenantId id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.DeleteAsync<TenantEntity>($"{TenantPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete tenant from Redis: {TenantId}", id.Value);
        }
    }

    public async Task<bool> ExistsAsync(TenantId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.KeyExistsAsync($"{TenantPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check tenant existence in Redis: {TenantId}", id.Value);
            return false;
        }
    }

    public Task<IReadOnlyList<TenantEntity>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<TenantEntity>>(new List<TenantEntity>());
    }

    public Task<TenantEntity?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<TenantEntity?>(null);
    }
}
