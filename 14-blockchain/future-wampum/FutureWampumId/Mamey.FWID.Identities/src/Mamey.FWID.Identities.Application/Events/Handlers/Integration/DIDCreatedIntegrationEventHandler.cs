using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.Events.Integration.DIDs;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Events.Handlers.Integration;

/// <summary>
/// Handler for DIDCreated integration event from DIDs service.
/// </summary>
internal sealed class DIDCreatedIntegrationEventHandler : IEventHandler<DIDCreatedIntegrationEvent>
{
    private readonly IDIDsServiceClient _didsServiceClient;
    private readonly ILogger<DIDCreatedIntegrationEventHandler> _logger;

    public DIDCreatedIntegrationEventHandler(
        IDIDsServiceClient didsServiceClient,
        ILogger<DIDCreatedIntegrationEventHandler> logger)
    {
        _didsServiceClient = didsServiceClient ?? throw new ArgumentNullException(nameof(didsServiceClient));
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

            // Verify the DID matches the event data
            if (did.DIDId != @event.DIDId || did.DidString != @event.DidString)
            {
                _logger.LogWarning(
                    "DID mismatch: Event DIDId={EventDIDId}, Service DIDId={ServiceDIDId}, Event DidString={EventDidString}, Service DidString={ServiceDidString}",
                    @event.DIDId, did.DIDId, @event.DidString, did.DidString);
                return;
            }

            // Handle integration event from DIDs service
            // For example: Update identity metadata with DID information, log the event, etc.
            _logger.LogInformation(
                "Processed DIDCreated integration event for IdentityId: {IdentityId}, DID: {DidString}",
                @event.IdentityId, did.DidString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling DIDCreated integration event for IdentityId: {IdentityId}", @event.IdentityId);
            throw;
        }
    }
}

