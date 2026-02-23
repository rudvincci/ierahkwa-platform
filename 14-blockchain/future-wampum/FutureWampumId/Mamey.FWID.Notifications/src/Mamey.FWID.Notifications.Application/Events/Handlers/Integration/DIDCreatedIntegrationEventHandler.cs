using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Application.Clients;
using Mamey.FWID.Notifications.Application.Events.Integration.DIDs;
using Mamey.FWID.Notifications.Application.Services;
using Mamey.FWID.Notifications.Application.Templates;
using Mamey.FWID.Notifications.Application.Templates.Models;
using Mamey.FWID.Notifications.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Repositories;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Application.Events.Handlers.Integration;

/// <summary>
/// Handler for DIDCreated integration event from DIDs service.
/// </summary>
internal sealed class DIDCreatedIntegrationEventHandler : IEventHandler<DIDCreatedIntegrationEvent>
{
    private readonly IDIDsServiceClient _didsServiceClient;
    private readonly IIdentitiesServiceClient _identitiesServiceClient;
    private readonly INotificationService _notificationService;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<DIDCreatedIntegrationEventHandler> _logger;

    public DIDCreatedIntegrationEventHandler(
        IDIDsServiceClient didsServiceClient,
        IIdentitiesServiceClient identitiesServiceClient,
        INotificationService notificationService,
        INotificationRepository notificationRepository,
        ILogger<DIDCreatedIntegrationEventHandler> logger)
    {
        _didsServiceClient = didsServiceClient ?? throw new ArgumentNullException(nameof(didsServiceClient));
        _identitiesServiceClient = identitiesServiceClient ?? throw new ArgumentNullException(nameof(identitiesServiceClient));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(DIDCreatedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Received DIDCreated integration event: DIDId={DIDId}, IdentityId={IdentityId}, DidString={DidString}",
            @event.DIDId, @event.IdentityId, @event.DidString);

        try
        {
            // Verify the DID exists by calling the DIDs service (source of truth)
            var did = await _didsServiceClient.GetDIDByIdentityIdAsync(@event.IdentityId, cancellationToken);
            
            if (did == null)
            {
                _logger.LogWarning("DID not found in DIDs service for IdentityId: {IdentityId}", @event.IdentityId);
                return;
            }

            // Get identity for email
            var identity = await _identitiesServiceClient.GetIdentityAsync(@event.IdentityId, cancellationToken);
            if (identity == null)
            {
                _logger.LogWarning("Identity not found for IdentityId: {IdentityId}", @event.IdentityId);
                return;
            }

            // Create DID creation notification
            var identityId = new IdentityId(@event.IdentityId);
            var notification = Notification.Create(
                identityId,
                "DID Created",
                "Your Decentralized Identifier has been created",
                $"Your DID has been successfully created: {did.DidString}",
                NotificationType.Email | NotificationType.InApp,
                "DID",
                @event.DIDId);

            // Store notification
            await _notificationRepository.AddAsync(notification, cancellationToken);

            // Send email if available
            if (!string.IsNullOrEmpty(identity.Email))
            {
                var name = Name.Parse(identity.Name);
                var emailModel = new DIDCreatedDto(name, did.DidString);
                await _notificationService.SendEmailUsingTemplate(
                    identity.Email,
                    "Your DID Has Been Created",
                    EmailTemplateType.DIDCreated,
                    emailModel);
            }

            // Send notification
            await _notificationService.SendAsync(notification);
            notification.MarkAsSent();

            await _notificationRepository.UpdateAsync(notification, cancellationToken);

            _logger.LogInformation(
                "Processed DIDCreated integration event and sent notification for IdentityId: {IdentityId}, DID: {DidString}",
                @event.IdentityId, did.DidString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling DIDCreated integration event for IdentityId: {IdentityId}", @event.IdentityId);
            throw;
        }
    }
}







