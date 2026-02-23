using Mamey.FWID.Notifications.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Application.Services;

/// <summary>
/// Implementation of notification storage service.
/// </summary>
internal sealed class NotificationStorageService : INotificationStorageService
{
    private readonly ILogger<NotificationStorageService> _logger;
    
    // In-memory storage for demo - use MongoDB repository in production
    private readonly Dictionary<Guid, Notification> _notifications = new();
    private readonly object _lock = new();
    
    public NotificationStorageService(ILogger<NotificationStorageService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc />
    public Task StoreAsync(Notification notification)
    {
        lock (_lock)
        {
            _notifications[notification.Id.Value] = notification;
        }
        
        _logger.LogDebug("Notification {NotificationId} stored", notification.Id);
        return Task.CompletedTask;
    }
    
    /// <inheritdoc />
    public Task UpdateAsync(Notification notification)
    {
        lock (_lock)
        {
            _notifications[notification.Id.Value] = notification;
        }
        
        _logger.LogDebug("Notification {NotificationId} updated", notification.Id);
        return Task.CompletedTask;
    }
    
    /// <inheritdoc />
    public Task<Notification?> GetByIdAsync(Guid id)
    {
        lock (_lock)
        {
            _notifications.TryGetValue(id, out var notification);
            return Task.FromResult(notification);
        }
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<Notification>> GetByIdentityIdAsync(Guid identityId, int limit = 50)
    {
        lock (_lock)
        {
            var notifications = _notifications.Values
                .Where(n => n.IdentityId.Value == identityId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(limit)
                .ToList();
            return Task.FromResult<IReadOnlyList<Notification>>(notifications);
        }
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<Notification>> GetUnreadByIdentityIdAsync(Guid identityId)
    {
        lock (_lock)
        {
            var notifications = _notifications.Values
                .Where(n => n.IdentityId.Value == identityId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
            return Task.FromResult<IReadOnlyList<Notification>>(notifications);
        }
    }
    
    /// <inheritdoc />
    public Task<int> GetUnreadCountAsync(Guid identityId)
    {
        lock (_lock)
        {
            var count = _notifications.Values
                .Count(n => n.IdentityId.Value == identityId && !n.IsRead);
            return Task.FromResult(count);
        }
    }
    
    /// <inheritdoc />
    public Task MarkAsReadAsync(Guid notificationId)
    {
        lock (_lock)
        {
            if (_notifications.TryGetValue(notificationId, out var notification))
            {
                notification.MarkAsRead();
            }
        }
        return Task.CompletedTask;
    }
    
    /// <inheritdoc />
    public Task MarkAllAsReadAsync(Guid identityId)
    {
        lock (_lock)
        {
            foreach (var notification in _notifications.Values
                .Where(n => n.IdentityId.Value == identityId && !n.IsRead))
            {
                notification.MarkAsRead();
            }
        }
        return Task.CompletedTask;
    }
}
