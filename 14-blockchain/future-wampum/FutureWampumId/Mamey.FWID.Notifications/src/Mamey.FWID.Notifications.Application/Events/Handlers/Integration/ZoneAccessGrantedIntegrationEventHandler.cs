using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Application.Clients;
using Mamey.FWID.Notifications.Application.Events.Integration.AccessControls;
using Mamey.FWID.Notifications.Application.Services;
using Mamey.FWID.Notifications.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Application.Events.Handlers.Integration;

/// <summary>
/// Handler for ZoneAccessGranted integration event from AccessControls service.
/// </summary>
internal sealed class ZoneAccessGrantedIntegrationEventHandler : IEventHandler<ZoneAccessGrantedIntegrationEvent>
{
    private readonly IAccessControlsServiceClient _accessControlsServiceClient;
    private readonly IIdentitiesServiceClient _identitiesServiceClient;
    private readonly INotificationService _notificationService;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<ZoneAccessGrantedIntegrationEventHandler> _logger;

    public ZoneAccessGrantedIntegrationEventHandler(
        IAccessControlsServiceClient accessControlsServiceClient,
        IIdentitiesServiceClient identitiesServiceClient,
        INotificationService notificationService,
        INotificationRepository notificationRepository,
        ILogger<ZoneAccessGrantedIntegrationEventHandler> logger)
    {
        _accessControlsServiceClient = accessControlsServiceClient ?? throw new ArgumentNullException(nameof(accessControlsServiceClient));
        _identitiesServiceClient = identitiesServiceClient ?? throw new ArgumentNullException(nameof(identitiesServiceClient));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(ZoneAccessGrantedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Received ZoneAccessGranted integration event: AccessControlId={AccessControlId}, IdentityId={IdentityId}, ZoneId={ZoneId}, Permission={Permission}",
            @event.AccessControlId, @event.IdentityId, @event.ZoneId, @event.Permission);

        try
        {
            // Verify the access control exists by calling the AccessControls service (source of truth)
            var accessControl = await _accessControlsServiceClient.GetAccessControlAsync(@event.AccessControlId, cancellationToken);
            
            if (accessControl == null)
            {
                _logger.LogWarning("Access control not found in AccessControls service for AccessControlId: {AccessControlId}", @event.AccessControlId);
                return;
            }

            // Get identity for email
            var identity = await _identitiesServiceClient.GetIdentityAsync(@event.IdentityId, cancellationToken);
            if (identity == null)
            {
                _logger.LogWarning("Identity not found for IdentityId: {IdentityId}", @event.IdentityId);
                return;
            }

            // Create access granted notification
            var identityId = new IdentityId(@event.IdentityId);
            var notification = Notification.Create(
                identityId,
                "Zone Access Granted",
                "Access to Zone Granted",
                $"You have been granted {accessControl.Permission} access to zone {accessControl.ZoneId}",
                NotificationType.Email | NotificationType.InApp,
                "AccessControl",
                @event.AccessControlId);

            // Store notification
            await _notificationRepository.AddAsync(notification, cancellationToken);

            // Send notification
            await _notificationService.SendAsync(notification);
            notification.MarkAsSent();

            await _notificationRepository.UpdateAsync(notification, cancellationToken);

            _logger.LogInformation(
                "Processed ZoneAccessGranted integration event and sent notification for IdentityId: {IdentityId}, AccessControlId: {AccessControlId}",
                @event.IdentityId, @event.AccessControlId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ZoneAccessGranted integration event for IdentityId: {IdentityId}", @event.IdentityId);
            throw;
        }
    }
}







