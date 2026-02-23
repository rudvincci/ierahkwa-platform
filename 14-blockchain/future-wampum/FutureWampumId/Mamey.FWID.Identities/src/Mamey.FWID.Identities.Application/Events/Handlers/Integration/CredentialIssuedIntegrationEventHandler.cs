using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.Events.Integration.Credentials;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Events.Handlers.Integration;

/// <summary>
/// Handler for CredentialIssued integration event from Credentials service.
/// </summary>
internal sealed class CredentialIssuedIntegrationEventHandler : IEventHandler<CredentialIssuedIntegrationEvent>
{
    private readonly ICredentialsServiceClient _credentialsServiceClient;
    private readonly ILogger<CredentialIssuedIntegrationEventHandler> _logger;

    public CredentialIssuedIntegrationEventHandler(
        ICredentialsServiceClient credentialsServiceClient,
        ILogger<CredentialIssuedIntegrationEventHandler> logger)
    {
        _credentialsServiceClient = credentialsServiceClient ?? throw new ArgumentNullException(nameof(credentialsServiceClient));
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

            // Verify the credential matches the event data
            if (credential.IdentityId != @event.IdentityId || credential.CredentialType != @event.CredentialType)
            {
                _logger.LogWarning(
                    "Credential mismatch: Event IdentityId={EventIdentityId}, Service IdentityId={ServiceIdentityId}, Event CredentialType={EventCredentialType}, Service CredentialType={ServiceCredentialType}",
                    @event.IdentityId, credential.IdentityId, @event.CredentialType, credential.CredentialType);
                return;
            }

            // Handle integration event from Credentials service
            // For example: Update identity metadata with credential information, log the event, etc.
            _logger.LogInformation(
                "Processed CredentialIssued integration event for IdentityId: {IdentityId}, CredentialId: {CredentialId}",
                @event.IdentityId, credential.CredentialId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling CredentialIssued integration event for IdentityId: {IdentityId}", @event.IdentityId);
            throw;
        }
    }
}

