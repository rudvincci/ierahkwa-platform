using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.Events.Integration.AccessControls;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Events.Handlers.Integration;

/// <summary>
/// Handler for ZoneAccessGranted integration event from AccessControls service.
/// </summary>
internal sealed class ZoneAccessGrantedIntegrationEventHandler : IEventHandler<ZoneAccessGrantedIntegrationEvent>
{
    private readonly IAccessControlsServiceClient _accessControlsServiceClient;
    private readonly ILogger<ZoneAccessGrantedIntegrationEventHandler> _logger;

    public ZoneAccessGrantedIntegrationEventHandler(
        IAccessControlsServiceClient accessControlsServiceClient,
        ILogger<ZoneAccessGrantedIntegrationEventHandler> logger)
    {
        _accessControlsServiceClient = accessControlsServiceClient ?? throw new ArgumentNullException(nameof(accessControlsServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(ZoneAccessGrantedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Received ZoneAccessGranted integration event: AccessControlId={AccessControlId}, IdentityId={IdentityId}, ZoneId={ZoneId}, Permission={Permission}",
            @event.AccessControlId, @event.IdentityId, @event.ZoneId, @event.Permission);

        try
        {
            // Verify the zone access exists by calling the AccessControls service (source of truth)
            var hasAccess = await _accessControlsServiceClient.CheckZoneAccessAsync(@event.IdentityId, @event.ZoneId, cancellationToken);
            
            if (!hasAccess)
            {
                _logger.LogWarning("Zone access not found in AccessControls service for IdentityId: {IdentityId}, ZoneId: {ZoneId}", 
                    @event.IdentityId, @event.ZoneId);
                return;
            }

            // Get all access controls for the identity to verify the permission
            var accessControls = await _accessControlsServiceClient.GetAccessControlsByIdentityIdAsync(@event.IdentityId, cancellationToken);
            var accessControl = accessControls.FirstOrDefault(ac => ac.ZoneId == @event.ZoneId);
            
            if (accessControl == null)
            {
                _logger.LogWarning("Access control not found for IdentityId: {IdentityId}, ZoneId: {ZoneId}", 
                    @event.IdentityId, @event.ZoneId);
                return;
            }

            // Verify the permission matches the event data
            if (accessControl.Permission != @event.Permission)
            {
                _logger.LogWarning(
                    "Permission mismatch: Event Permission={EventPermission}, Service Permission={ServicePermission}",
                    @event.Permission, accessControl.Permission);
                return;
            }

            // Handle integration event from AccessControls service
            // For example: Update identity zone information, log the event, etc.
            _logger.LogInformation(
                "Processed ZoneAccessGranted integration event for IdentityId: {IdentityId}, ZoneId: {ZoneId}, Permission: {Permission}",
                @event.IdentityId, @event.ZoneId, accessControl.Permission);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ZoneAccessGranted integration event for IdentityId: {IdentityId}", @event.IdentityId);
            throw;
        }
    }
}

