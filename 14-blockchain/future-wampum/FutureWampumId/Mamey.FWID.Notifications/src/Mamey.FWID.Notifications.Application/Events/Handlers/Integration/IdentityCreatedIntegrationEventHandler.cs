using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Application.Clients;
using Mamey.FWID.Notifications.Application.Events.Integration.Identities;
using Mamey.FWID.Notifications.Application.Services;
using Mamey.FWID.Notifications.Application.Templates;
using Mamey.FWID.Notifications.Application.Templates.Models;
using Mamey.FWID.Notifications.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Repositories;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Application.Events.Handlers.Integration;

/// <summary>
/// Handler for IdentityCreated integration event from Identities service.
/// </summary>
internal sealed class IdentityCreatedIntegrationEventHandler : IEventHandler<IdentityCreatedIntegrationEvent>
{
    private readonly IIdentitiesServiceClient _identitiesServiceClient;
    private readonly INotificationService _notificationService;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<IdentityCreatedIntegrationEventHandler> _logger;

    public IdentityCreatedIntegrationEventHandler(
        IIdentitiesServiceClient identitiesServiceClient,
        INotificationService notificationService,
        INotificationRepository notificationRepository,
        ILogger<IdentityCreatedIntegrationEventHandler> logger)
    {
        _identitiesServiceClient = identitiesServiceClient ?? throw new ArgumentNullException(nameof(identitiesServiceClient));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(IdentityCreatedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Received IdentityCreated integration event: IdentityId={IdentityId}, Name={Name}, Zone={Zone}",
            @event.IdentityId, @event.Name, @event.Zone);

        try
        {
            // Verify the identity exists by calling the Identities service (source of truth)
            var identity = await _identitiesServiceClient.GetIdentityAsync(@event.IdentityId, cancellationToken);
            
            if (identity == null)
            {
                _logger.LogWarning("Identity not found in Identities service for IdentityId: {IdentityId}", @event.IdentityId);
                return;
            }

            // Create welcome notification
            var identityId = new IdentityId(@event.IdentityId);
            var notification = Notification.Create(
                identityId,
                "Welcome to FutureWampumID",
                "Identity Created",
                $"Welcome {identity.Name}! Your identity has been successfully created.",
                NotificationType.Email | NotificationType.InApp,
                "Identity",
                @event.IdentityId);

            // Store notification
            await _notificationRepository.AddAsync(notification, cancellationToken);

            // Send welcome email if email is available
            if (!string.IsNullOrEmpty(identity.Email))
            {
                var name = Name.Parse(identity.Name);
                var emailModel = new WelcomeDto(name, "verification-token-placeholder");
                await _notificationService.SendEmailUsingTemplate(
                    identity.Email,
                    "Welcome to FutureWampumID",
                    EmailTemplateType.Welcome,
                    emailModel);
            }

            // Send notification
            await _notificationService.SendAsync(notification);
            notification.MarkAsSent();

            await _notificationRepository.UpdateAsync(notification, cancellationToken);

            _logger.LogInformation(
                "Processed IdentityCreated integration event and sent welcome notification for IdentityId: {IdentityId}",
                @event.IdentityId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling IdentityCreated integration event for IdentityId: {IdentityId}", @event.IdentityId);
            throw;
        }
    }
}







