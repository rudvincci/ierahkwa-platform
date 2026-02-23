using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Types;
using Mamey.Persistence.Redis;
using Microsoft.Extensions.Logging;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Redis.Repositories;

internal class ApplicationRedisRepository : IApplicationRepository
{
    private readonly ICache _cache;
    private readonly ILogger<ApplicationRedisRepository> _logger;
    private const string ApplicationPrefix = "citizenship-applications:applications:";

    public ApplicationRedisRepository(
        ICache cache,
        ILogger<ApplicationRedisRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<CitizenshipApplication?> GetAsync(AppId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.GetAsync<CitizenshipApplication>($"{ApplicationPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get application from Redis: {ApplicationId}", id.Value);
            return null;
        }
    }

    public async Task AddAsync(CitizenshipApplication application, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.SetAsync($"{ApplicationPrefix}{application.Id.Value}", application, TimeSpan.FromHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add application to Redis: {ApplicationId}", application.Id.Value);
        }
    }

    public async Task UpdateAsync(CitizenshipApplication application, CancellationToken cancellationToken = default)
    {
        await AddAsync(application, cancellationToken);
    }

    public async Task DeleteAsync(AppId id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.DeleteAsync<CitizenshipApplication>($"{ApplicationPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete application from Redis: {ApplicationId}", id.Value);
        }
    }

    public async Task<bool> ExistsAsync(AppId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.KeyExistsAsync($"{ApplicationPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check application existence in Redis: {ApplicationId}", id.Value);
            return false;
        }
    }

    public Task<IReadOnlyList<CitizenshipApplication>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<CitizenshipApplication>>(new List<CitizenshipApplication>());
    }

    public Task<IList<CitizenshipApplication>> GetAllByApplicationEmail(string email, CancellationToken cancellationToken = default)
    {
        // Redis doesn't support efficient querying by email property
        // Return empty list to fall through to Postgres/Mongo
        return Task.FromResult<IList<CitizenshipApplication>>(new List<CitizenshipApplication>());
    }

    public Task<CitizenshipApplication?> GetByApplicationEmail(string email, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<CitizenshipApplication?>(null);
    }

    public Task<CitizenshipApplication?> GetByApplicationNumberAsync(ApplicationNumber applicationNumber, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<CitizenshipApplication?>(null);
    }

    public Task<IReadOnlyList<CitizenshipApplication>> GetByTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<CitizenshipApplication>>(new List<CitizenshipApplication>());
    }

    public Task<IReadOnlyList<CitizenshipApplication>> GetByStatusAsync(ApplicationStatus status, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<CitizenshipApplication>>(new List<CitizenshipApplication>());
    }

    public Task<int> GetCountByYearAsync(TenantId tenantId, int year, CancellationToken cancellationToken = default)
    {
        // Redis doesn't support counting - return 0 to fall through to Postgres
        return Task.FromResult(0);
    }
}
