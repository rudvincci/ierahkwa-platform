using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Entities;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;
using Mamey.Persistence.Redis;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.TravelIdentities.Core.Redis.Repositories;

internal class TravelIdentityRedisRepository : ITravelIdentityRepository
{
    private readonly ICache _cache;
    private readonly ILogger<TravelIdentityRedisRepository> _logger;
    private const string TravelIdentityPrefix = "travel-identities:travel-identities:";

    public TravelIdentityRedisRepository(
        ICache cache,
        ILogger<TravelIdentityRedisRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TravelIdentity?> GetAsync(TravelIdentityId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.GetAsync<TravelIdentity>($"{TravelIdentityPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get travel identity from Redis: {TravelIdentityId}", id.Value);
            return null;
        }
    }

    public async Task AddAsync(TravelIdentity travelIdentity, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.SetAsync($"{TravelIdentityPrefix}{travelIdentity.Id.Value}", travelIdentity, TimeSpan.FromHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add travel identity to Redis: {TravelIdentityId}", travelIdentity.Id.Value);
        }
    }

    public async Task UpdateAsync(TravelIdentity travelIdentity, CancellationToken cancellationToken = default)
    {
        await AddAsync(travelIdentity, cancellationToken);
    }

    public async Task DeleteAsync(TravelIdentityId id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.DeleteAsync<TravelIdentity>($"{TravelIdentityPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete travel identity from Redis: {TravelIdentityId}", id.Value);
        }
    }

    public async Task<bool> ExistsAsync(TravelIdentityId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.KeyExistsAsync($"{TravelIdentityPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check travel identity existence in Redis: {TravelIdentityId}", id.Value);
            return false;
        }
    }

    public Task<IReadOnlyList<TravelIdentity>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<TravelIdentity>>(new List<TravelIdentity>());
    }

    public Task<TravelIdentity?> GetByTravelIdentityNumberAsync(TravelIdentityNumber travelIdentityNumber, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<TravelIdentity?>(null);
    }

    public Task<IReadOnlyList<TravelIdentity>> GetByCitizenAsync(CitizenId citizenId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<TravelIdentity>>(new List<TravelIdentity>());
    }

    public Task<IReadOnlyList<TravelIdentity>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<TravelIdentity>>(new List<TravelIdentity>());
    }

    public Task<int> GetCountByYearAsync(TenantId tenantId, int year, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
