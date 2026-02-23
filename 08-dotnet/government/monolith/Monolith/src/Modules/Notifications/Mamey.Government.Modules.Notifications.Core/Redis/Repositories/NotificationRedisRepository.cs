using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Government.Modules.Notifications.Core.Domain.Repositories;
using Mamey.Government.Modules.Notifications.Core.Domain.Types;
using Mamey.Persistence.Redis;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Notifications.Core.Redis.Repositories;

internal class NotificationRedisRepository : INotificationRepository
{
    private readonly ICache _cache;
    private readonly ILogger<NotificationRedisRepository> _logger;
    private const string NotificationPrefix = "notifications:";

    public NotificationRedisRepository(
        ICache cache,
        ILogger<NotificationRedisRepository> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<Notification?> GetAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.GetAsync<Notification>($"{NotificationPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get notification from Redis: {NotificationId}", id.Value);
            return null;
        }
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        try
        {
            // Cache for 1 hour
            await _cache.SetAsync($"{NotificationPrefix}{notification.Id.Value}", notification, TimeSpan.FromHours(1));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to add notification to Redis: {NotificationId}", notification.Id.Value);
        }
    }

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await AddAsync(notification, cancellationToken);
    }

    public async Task DeleteAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _cache.DeleteAsync<Notification>($"{NotificationPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete notification from Redis: {NotificationId}", id.Value);
        }
    }

    public async Task<bool> ExistsAsync(NotificationId id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cache.KeyExistsAsync($"{NotificationPrefix}{id.Value}");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check notification existence in Redis: {NotificationId}", id.Value);
            return false;
        }
    }

    public Task<IReadOnlyList<Notification>> BrowseAsync(CancellationToken cancellationToken = default)
    {
        // Redis doesn't support complex queries - return empty list to fall through to Mongo/Postgres
        return Task.FromResult<IReadOnlyList<Notification>>(new List<Notification>());
    }
}
