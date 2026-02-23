using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Passports.Core.Domain.Entities;
using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Mamey.Government.Modules.Passports.Core.Domain.ValueObjects;
using Mamey.Types;
using Mamey.Persistence.Redis;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Passports.Core.Redis.Repositories;

internal class PassportRedisRepository : IPassportRepository
{
    private readonly ICache _cache;
    private readonly ILogger<PassportRedisRepository> _logger;
    private const string PassportPrefix = "passports:passports:";

    public PassportRedisRepository(
        ICache cache,
        ILogger<PassportRedisRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<Passport?> GetAsync(PassportId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.GetAsync<Passport>($"{PassportPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get passport from Redis: {PassportId}", id.Value);
            return null;
        }
    }

    public async Task AddAsync(Passport passport, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.SetAsync($"{PassportPrefix}{passport.Id.Value}", passport, TimeSpan.FromHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add passport to Redis: {PassportId}", passport.Id.Value);
        }
    }

    public async Task UpdateAsync(Passport passport, CancellationToken cancellationToken = default)
    {
        await AddAsync(passport, cancellationToken);
    }

    public async Task DeleteAsync(PassportId id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.DeleteAsync<Passport>($"{PassportPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete passport from Redis: {PassportId}", id.Value);
        }
    }

    public async Task<bool> ExistsAsync(PassportId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.KeyExistsAsync($"{PassportPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check passport existence in Redis: {PassportId}", id.Value);
            return false;
        }
    }

    public Task<IReadOnlyList<Passport>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Passport>>(new List<Passport>());
    }

    public Task<Passport?> GetByPassportNumberAsync(PassportNumber passportNumber, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Passport?>(null);
    }

    public Task<IReadOnlyList<Passport>> GetByCitizenAsync(CitizenId citizenId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Passport>>(new List<Passport>());
    }

    public Task<IReadOnlyList<Passport>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Passport>>(new List<Passport>());
    }
}
