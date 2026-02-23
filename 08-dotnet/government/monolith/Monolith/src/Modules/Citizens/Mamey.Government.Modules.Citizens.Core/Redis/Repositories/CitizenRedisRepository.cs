using Mamey.Government.Modules.Citizens.Core.Domain.Entities;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Types;
using Mamey.Persistence.Redis;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Citizens.Core.Redis.Repositories;

internal class CitizenRedisRepository : ICitizenRepository
{
    private readonly ICache _cache;
    private readonly ILogger<CitizenRedisRepository> _logger;
    private const string CitizenPrefix = "citizens:citizens:";

    public CitizenRedisRepository(
        ICache cache,
        ILogger<CitizenRedisRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<Citizen?> GetAsync(CitizenId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.GetAsync<Citizen>($"{CitizenPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get citizen from Redis: {CitizenId}", id.Value);
            return null;
        }
    }

    public async Task AddAsync(Citizen citizen, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.SetAsync($"{CitizenPrefix}{citizen.Id.Value}", citizen, TimeSpan.FromHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add citizen to Redis: {CitizenId}", citizen.Id.Value);
        }
    }

    public async Task UpdateAsync(Citizen citizen, CancellationToken cancellationToken = default)
    {
        await AddAsync(citizen, cancellationToken);
    }

    public async Task DeleteAsync(CitizenId id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.DeleteAsync<Citizen>($"{CitizenPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete citizen from Redis: {CitizenId}", id.Value);
        }
    }

    public async Task<bool> ExistsAsync(CitizenId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.KeyExistsAsync($"{CitizenPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check citizen existence in Redis: {CitizenId}", id.Value);
            return false;
        }
    }

    public Task<IReadOnlyList<Citizen>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Citizen>>(new List<Citizen>());
    }

    public Task<IReadOnlyList<Citizen>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Citizen>>(new List<Citizen>());
    }

    public Task<IReadOnlyList<Citizen>> GetByStatusAsync(CitizenshipStatus status, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Citizen>>(new List<Citizen>());
    }
}
