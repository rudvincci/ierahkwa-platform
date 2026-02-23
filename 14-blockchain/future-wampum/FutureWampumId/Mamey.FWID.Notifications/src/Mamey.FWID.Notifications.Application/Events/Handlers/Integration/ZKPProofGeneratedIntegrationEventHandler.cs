using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Application.Clients;
using Mamey.FWID.Notifications.Application.Events.Integration.ZKPs;
using Mamey.FWID.Notifications.Application.Services;
using Mamey.FWID.Notifications.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Application.Events.Handlers.Integration;

/// <summary>
/// Handler for ZKPProofGenerated integration event from ZKPs service.
/// </summary>
internal sealed class ZKPProofGeneratedIntegrationEventHandler : IEventHandler<ZKPProofGeneratedIntegrationEvent>
{
    private readonly IZKPsServiceClient _zkpsServiceClient;
    private readonly IIdentitiesServiceClient _identitiesServiceClient;
    private readonly INotificationService _notificationService;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<ZKPProofGeneratedIntegrationEventHandler> _logger;

    public ZKPProofGeneratedIntegrationEventHandler(
        IZKPsServiceClient zkpsServiceClient,
        IIdentitiesServiceClient identitiesServiceClient,
        INotificationService notificationService,
        INotificationRepository notificationRepository,
        ILogger<ZKPProofGeneratedIntegrationEventHandler> logger)
    {
        _zkpsServiceClient = zkpsServiceClient ?? throw new ArgumentNullException(nameof(zkpsServiceClient));
        _identitiesServiceClient = identitiesServiceClient ?? throw new ArgumentNullException(nameof(identitiesServiceClient));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(ZKPProofGeneratedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Received ZKPProofGenerated integration event: ProofId={ProofId}, IdentityId={IdentityId}, AttributeType={AttributeType}",
            @event.ProofId, @event.IdentityId, @event.AttributeType);

        try
        {
            // Verify the ZKP proof exists by calling the ZKPs service (source of truth)
            var proof = await _zkpsServiceClient.GetZKPProofAsync(@event.ProofId, cancellationToken);
            
            if (proof == null)
            {
                _logger.LogWarning("ZKP proof not found in ZKPs service for ProofId: {ProofId}", @event.ProofId);
                return;
            }

            // Get identity for email
            var identity = await _identitiesServiceClient.GetIdentityAsync(@event.IdentityId, cancellationToken);
            if (identity == null)
            {
                _logger.LogWarning("Identity not found for IdentityId: {IdentityId}", @event.IdentityId);
                return;
            }

            // Create ZKP proof notification
            var identityId = new IdentityId(@event.IdentityId);
            var notification = Notification.Create(
                identityId,
                "ZKP Proof Generated",
                "Zero-Knowledge Proof Generated",
                $"A ZKP proof has been generated for attribute: {proof.AttributeType}",
                NotificationType.Email | NotificationType.InApp,
                "ZKP",
                @event.ProofId);

            // Store notification
            await _notificationRepository.AddAsync(notification, cancellationToken);

            // Send notification
            await _notificationService.SendAsync(notification);
            notification.MarkAsSent();

            await _notificationRepository.UpdateAsync(notification, cancellationToken);

            _logger.LogInformation(
                "Processed ZKPProofGenerated integration event and sent notification for IdentityId: {IdentityId}, ProofId: {ProofId}",
                @event.IdentityId, @event.ProofId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ZKPProofGenerated integration event for IdentityId: {IdentityId}", @event.IdentityId);
            throw;
        }
    }
}







