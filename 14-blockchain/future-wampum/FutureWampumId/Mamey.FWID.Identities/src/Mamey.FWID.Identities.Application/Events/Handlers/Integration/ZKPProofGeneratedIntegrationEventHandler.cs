using Mamey.CQRS.Events;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.Events.Integration.ZKPs;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Events.Handlers.Integration;

/// <summary>
/// Handler for ZKPProofGenerated integration event from ZKPs service.
/// </summary>
internal sealed class ZKPProofGeneratedIntegrationEventHandler : IEventHandler<ZKPProofGeneratedIntegrationEvent>
{
    private readonly IZKPsServiceClient _zkpsServiceClient;
    private readonly ILogger<ZKPProofGeneratedIntegrationEventHandler> _logger;

    public ZKPProofGeneratedIntegrationEventHandler(
        IZKPsServiceClient zkpsServiceClient,
        ILogger<ZKPProofGeneratedIntegrationEventHandler> logger)
    {
        _zkpsServiceClient = zkpsServiceClient ?? throw new ArgumentNullException(nameof(zkpsServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(ZKPProofGeneratedIntegrationEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Received ZKPProofGenerated integration event: ProofId={ProofId}, IdentityId={IdentityId}, AttributeType={AttributeType}",
            @event.ProofId, @event.IdentityId, @event.AttributeType);

        try
        {
            // Verify the ZKP proof exists by calling the ZKPs service (source of truth)
            // Note: ZKPs service may not have GetProofById, so we'll get all proofs for the identity
            var proofs = await _zkpsServiceClient.GetZKPProofsByIdentityIdAsync(@event.IdentityId, cancellationToken);
            
            var proof = proofs.FirstOrDefault(p => p.ProofId == @event.ProofId);
            if (proof == null)
            {
                _logger.LogWarning("ZKP proof not found in ZKPs service for ProofId: {ProofId}", @event.ProofId);
                return;
            }

            // Verify the proof matches the event data
            if (proof.AttributeType != @event.AttributeType)
            {
                _logger.LogWarning(
                    "ZKP proof mismatch: Event AttributeType={EventAttributeType}, Service AttributeType={ServiceAttributeType}",
                    @event.AttributeType, proof.AttributeType);
                return;
            }

            // Handle integration event from ZKPs service
            // For example: Update identity metadata with ZKP proof information, log the event, etc.
            _logger.LogInformation(
                "Processed ZKPProofGenerated integration event for IdentityId: {IdentityId}, ProofId: {ProofId}",
                @event.IdentityId, proof.ProofId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ZKPProofGenerated integration event for IdentityId: {IdentityId}", @event.IdentityId);
            throw;
        }
    }
}

