using Mamey.Government.Modules.Identity.Core.Domain.Entities;
using Mamey.Government.Modules.Identity.Core.Domain.Repositories;
using Mamey.Persistence.Redis;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Identity.Core.Redis.Repositories;

internal class UserProfileRedisRepository : IUserProfileRepository
{
    private readonly ICache _cache;
    private readonly ILogger<UserProfileRedisRepository> _logger;
    private const string UserProfilePrefix = "identity:userprofiles:";

    public UserProfileRedisRepository(
        ICache cache,
        ILogger<UserProfileRedisRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<UserProfile?> GetAsync(UserId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.GetAsync<UserProfile>($"{UserProfilePrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get user profile from Redis: {UserId}", id.Value);
            return null;
        }
    }

    public async Task AddAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        try
        {
            // Cache for 1 hour
            await _cache.SetAsync($"{UserProfilePrefix}{userProfile.Id.Value}", userProfile, TimeSpan.FromHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add user profile to Redis: {UserId}", userProfile.Id.Value);
        }
    }

    public async Task UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        await AddAsync(userProfile, cancellationToken);
    }

    public async Task DeleteAsync(UserId id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.DeleteAsync<UserProfile>($"{UserProfilePrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete user profile from Redis: {UserId}", id.Value);
        }
    }

    public async Task<bool> ExistsAsync(UserId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.KeyExistsAsync($"{UserProfilePrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check user profile existence in Redis: {UserId}", id.Value);
            return false;
        }
    }

    public Task<IReadOnlyList<UserProfile>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        // Redis doesn't support complex queries - return empty list to fall through to Mongo/Postgres
        return Task.FromResult<IReadOnlyList<UserProfile>>(new List<UserProfile>());
    }

    public Task<UserProfile?> GetByAuthenticatorAsync(string issuer, string subject, CancellationToken cancellationToken = default)
    {
        // Redis doesn't support secondary indexes - return null to fall through to Mongo/Postgres
        return Task.FromResult<UserProfile?>(null);
    }

    public Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        // Redis doesn't support secondary indexes - return null to fall through to Mongo/Postgres
        return Task.FromResult<UserProfile?>(null);
    }
}
