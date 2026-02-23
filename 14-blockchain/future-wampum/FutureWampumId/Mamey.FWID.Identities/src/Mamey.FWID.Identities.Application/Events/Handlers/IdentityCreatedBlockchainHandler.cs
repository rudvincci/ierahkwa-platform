using Mamey.Contexts;
using Mamey.CQRS;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.Microservice.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Events.Handlers;

/// <summary>
/// Event handler that registers IdentityCreated events on the MameyNode Government blockchain.
/// Creates a permanent, immutable sovereign identity record.
/// 
/// TDD Reference: Line 1594-1703 (Feature Domain 1: Biometric Identity)
/// BDD Reference: Lines 86-112 (I.1-I.3 Executive Summary - Sovereign Identity)
/// </summary>
internal sealed class IdentityCreatedBlockchainHandler : IDomainEventHandler<IdentityCreated>
{
    private readonly IGovernmentIdentityService _governmentService;
    private readonly IIdentityRepository _identityRepository;
    private readonly IContext _context;
    private readonly ILogger<IdentityCreatedBlockchainHandler> _logger;

    public IdentityCreatedBlockchainHandler(
        IGovernmentIdentityService governmentService,
        IIdentityRepository identityRepository,
        IContext context,
        ILogger<IdentityCreatedBlockchainHandler> logger)
    {
        _governmentService = governmentService ?? throw new ArgumentNullException(nameof(governmentService));
        _identityRepository = identityRepository ?? throw new ArgumentNullException(nameof(identityRepository));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleAsync(IdentityCreated @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling IdentityCreated event for government blockchain registration: IdentityId={IdentityId}, Name={Name}",
            @event.IdentityId.Value, @event.Name?.FullName);

        try
        {
            // Retrieve the full identity entity for blockchain registration
            var identity = await _identityRepository.GetAsync(@event.IdentityId, cancellationToken);
            if (identity == null)
            {
                _logger.LogWarning(
                    "Identity not found for government blockchain registration: IdentityId={IdentityId}",
                    @event.IdentityId.Value);
                return;
            }

            // Check if identity is already registered on blockchain
            if (identity.Metadata.ContainsKey("GovernmentBlockchainId"))
            {
                _logger.LogDebug(
                    "Identity already registered on government blockchain: IdentityId={IdentityId}",
                    @event.IdentityId.Value);
                return;
            }

            // Register sovereign identity on MameyNode Government blockchain
            var result = await _governmentService.RegisterIdentityAsync(
                identity,
                _context.CorrelationId.ToString(),
                cancellationToken);

            if (result.Success && !string.IsNullOrEmpty(result.BlockchainIdentityId))
            {
                // Update identity metadata with blockchain registration info
                identity.Metadata["GovernmentBlockchainId"] = result.BlockchainIdentityId;
                identity.Metadata["GovernmentBlockchainRegisteredAt"] = DateTime.UtcNow.ToString("O");
                
                if (!string.IsNullOrEmpty(result.BlockchainAccount))
                {
                    identity.Metadata["GovernmentBlockchainAccount"] = result.BlockchainAccount;
                }

                await _identityRepository.UpdateAsync(identity, cancellationToken);

                _logger.LogInformation(
                    "Successfully registered identity on government blockchain: IdentityId={IdentityId}, BlockchainId={BlockchainId}",
                    @event.IdentityId.Value, result.BlockchainIdentityId);
            }
            else
            {
                // Record failure for potential retry
                identity.Metadata["GovernmentBlockchainRegistrationFailed"] = true;
                identity.Metadata["GovernmentBlockchainRegistrationError"] = result.ErrorMessage ?? "Unknown error";
                identity.Metadata["GovernmentBlockchainRegistrationFailedAt"] = DateTime.UtcNow.ToString("O");

                await _identityRepository.UpdateAsync(identity, cancellationToken);

                _logger.LogWarning(
                    "Failed to register identity on government blockchain: IdentityId={IdentityId}, Error={Error}",
                    @event.IdentityId.Value, result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the event handler - blockchain registration is best effort
            _logger.LogError(ex,
                "Error registering IdentityCreated event on government blockchain: IdentityId={IdentityId}",
                @event.IdentityId.Value);
        }
    }
}
