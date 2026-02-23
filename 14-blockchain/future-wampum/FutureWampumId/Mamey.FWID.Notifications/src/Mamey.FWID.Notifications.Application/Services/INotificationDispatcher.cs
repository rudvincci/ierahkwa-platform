using Mamey.FWID.Notifications.Domain.Entities;

namespace Mamey.FWID.Notifications.Application.Services;

/// <summary>
/// Interface for notification dispatch and persistence.
/// </summary>
public interface INotificationDispatcher
{
    /// <summary>
    /// Stores a notification.
    /// </summary>
    Task StoreAsync(Notification notification);
    
    /// <summary>
    /// Updates a notification.
    /// </summary>
    Task UpdateAsync(Notification notification);
    
    /// <summary>
    /// Gets a notification by ID.
    /// </summary>
    Task<Notification?> GetByIdAsync(Guid id);
    
    /// <summary>
    /// Gets notifications for an identity.
    /// </summary>
    Task<IReadOnlyList<Notification>> GetByIdentityIdAsync(Guid identityId, int limit = 50);
    
    /// <summary>
    /// Gets unread notifications for an identity.
    /// </summary>
    Task<IReadOnlyList<Notification>> GetUnreadByIdentityIdAsync(Guid identityId);
    
    /// <summary>
    /// Gets unread count for an identity.
    /// </summary>
    Task<int> GetUnreadCountAsync(Guid identityId);
    
    /// <summary>
    /// Marks a notification as read.
    /// </summary>
    Task MarkAsReadAsync(Guid notificationId);
    
    /// <summary>
    /// Marks all notifications as read for an identity.
    /// </summary>
    Task MarkAllAsReadAsync(Guid identityId);
}
