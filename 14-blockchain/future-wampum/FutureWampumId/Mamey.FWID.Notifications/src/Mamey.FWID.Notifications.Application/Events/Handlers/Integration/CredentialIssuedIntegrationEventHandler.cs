using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Notifications.Application.Clients;
using Mamey.FWID.Notifications.Application.Events.Integration.Credentials;
using Mamey.FWID.Notifications.Application.Services;
using Mamey.FWID.Notifications.Application.Templates;
using Mamey.FWID.Notifications.Application.Templates.Models;
using Mamey.FWID.Notifications.Domain.Entities;
using Mamey.FWID.Notifications.Domain.Repositories;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Notifications.Application.Events.Handlers.Integration;

/// <summary>
/// Handler for CredentialIssued integration event from Credentials service.
/// </summary>
internal sealed class CredentialIssuedIntegrationEventHandler : IEventHandler<CredentialIssuedIntegrationEvent>
{
    private readonly ICredentialsServiceClient _credentialsServiceClient;
    private readonly IIdentitiesServiceClient _identitiesServiceClient;
    private readonly INotificationService _notificationService;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<CredentialIssuedIntegrationEventHandler> _logger;

    public CredentialIssuedIntegrationEventHandler(
        ICredentialsServiceClient credentialsServiceClient,
        IIdentitiesServiceClient identitiesServiceClient,
        INotificationService notificationService,
        INotificationRepository notificationRepository,
        ILogger<CredentialIssuedIntegrationEventHandler> logger)
    {
        _credentialsServiceClient = credentialsServiceClient ?? throw new ArgumentNullException(nameof(credentialsServiceClient));
        _identitiesServiceClient = identitiesServiceClient ?? throw new ArgumentNullException(nameof(identitiesServiceClient));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(CredentialIssuedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Received CredentialIssued integration event: CredentialId={CredentialId}, IdentityId={IdentityId}, CredentialType={CredentialType}",
            @event.CredentialId, @event.IdentityId, @event.CredentialType);

        try
        {
            // Verify the credential exists by calling the Credentials service (source of truth)
            var credential = await _credentialsServiceClient.GetCredentialAsync(@event.CredentialId, cancellationToken);
            
            if (credential == null)
            {
                _logger.LogWarning("Credential not found in Credentials service for CredentialId: {CredentialId}", @event.CredentialId);
                return;
            }

            // Get identity for email
            var identity = await _identitiesServiceClient.GetIdentityAsync(@event.IdentityId, cancellationToken);
            if (identity == null)
            {
                _logger.LogWarning("Identity not found for IdentityId: {IdentityId}", @event.IdentityId);
                return;
            }

            // Create credential issuance notification
            var identityId = new IdentityId(@event.IdentityId);
            var notification = Notification.Create(
                identityId,
                "Credential Issued",
                "New Credential Issued",
                $"A new credential has been issued: {credential.CredentialType}",
                NotificationType.Email | NotificationType.InApp,
                "Credential",
                @event.CredentialId);

            // Store notification
            await _notificationRepository.AddAsync(notification, cancellationToken);

            // Send email if available
            if (!string.IsNullOrEmpty(identity.Email))
            {
                var name = Name.Parse(identity.Name);
                var emailModel = new CredentialIssuedDto(name, credential.CredentialType);
                await _notificationService.SendEmailUsingTemplate(
                    identity.Email,
                    "Credential Issued",
                    EmailTemplateType.CredentialIssued,
                    emailModel);
            }

            // Send notification
            await _notificationService.SendAsync(notification);
            notification.MarkAsSent();

            await _notificationRepository.UpdateAsync(notification, cancellationToken);

            _logger.LogInformation(
                "Processed CredentialIssued integration event and sent notification for IdentityId: {IdentityId}, CredentialId: {CredentialId}",
                @event.IdentityId, @event.CredentialId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling CredentialIssued integration event for IdentityId: {IdentityId}", @event.IdentityId);
            throw;
        }
    }
}







