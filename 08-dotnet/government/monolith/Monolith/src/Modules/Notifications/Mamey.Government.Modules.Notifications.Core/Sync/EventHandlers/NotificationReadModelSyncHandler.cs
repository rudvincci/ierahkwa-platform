using Mamey.CQRS;
using Mamey.Government.Modules.Notifications.Core.Domain.Events;
using Mamey.Government.Modules.Notifications.Core.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Notifications.Core.Sync.EventHandlers;

/// <summary>
/// Event handler for real-time synchronization of notification changes from PostgreSQL to MongoDB.
/// Provides event-driven replication for CQRS consistency.
/// </summary>
internal sealed class NotificationReadModelSyncHandler : 
    IDomainEventHandler<NotificationCreated>,
    IDomainEventHandler<NotificationModified>,
    IDomainEventHandler<NotificationRemoved>
{
    private readonly IReadModelSyncService _syncService;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationReadModelSyncHandler> _logger;

    public NotificationReadModelSyncHandler(
        IReadModelSyncService syncService,
        INotificationRepository notificationRepository,
        ILogger<NotificationReadModelSyncHandler> logger)
    {
        _syncService = syncService;
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public async Task HandleAsync(NotificationCreated @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling NotificationCreated event for read model sync: NotificationId={NotificationId}",
            @event.NotificationId.Value);

        try
        {
            // Fetch the full notification from PostgreSQL
            var notification = await _notificationRepository.GetAsync(@event.NotificationId, cancellationToken);
            if (notification == null)
            {
                _logger.LogWarning(
                    "Notification not found for read model sync: NotificationId={NotificationId}",
                    @event.NotificationId.Value);
                return;
            }

            // Sync to MongoDB read model
            await _syncService.SyncNotificationAsync(notification, cancellationToken);
            
            _logger.LogInformation(
                "Successfully synced NotificationCreated to MongoDB read model: NotificationId={NotificationId}",
                @event.NotificationId.Value);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the event handler - sync failures are logged but don't block domain events
            _logger.LogError(ex,
                "Error syncing NotificationCreated to MongoDB read model: NotificationId={NotificationId}",
                @event.NotificationId.Value);
        }
    }

    public async Task HandleAsync(NotificationModified @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling NotificationModified event for read model sync: NotificationId={NotificationId}",
            @event.Notification.Id.Value);

        try
        {
            // The event contains the modified notification
            await _syncService.SyncNotificationAsync(@event.Notification, cancellationToken);
            
            _logger.LogInformation(
                "Successfully synced NotificationModified to MongoDB read model: NotificationId={NotificationId}",
                @event.Notification.Id.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error syncing NotificationModified to MongoDB read model: NotificationId={NotificationId}",
                @event.Notification.Id.Value);
        }
    }

    public async Task HandleAsync(NotificationRemoved @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling NotificationRemoved event for read model sync: NotificationId={NotificationId}",
            @event.Notification.Id.Value);

        try
        {
            // Remove from MongoDB read model
            await _syncService.RemoveNotificationAsync(@event.Notification.Id, cancellationToken);
            
            _logger.LogInformation(
                "Successfully synced NotificationRemoved to MongoDB read model: NotificationId={NotificationId}",
                @event.Notification.Id.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error syncing NotificationRemoved to MongoDB read model: NotificationId={NotificationId}",
                @event.Notification.Id.Value);
        }
    }
}
