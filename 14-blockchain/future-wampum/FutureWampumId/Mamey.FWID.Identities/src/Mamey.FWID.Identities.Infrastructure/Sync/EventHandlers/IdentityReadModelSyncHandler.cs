using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Infrastructure.Sync.EventHandlers;

/// <summary>
/// Event handler for real-time synchronization of identity changes from PostgreSQL to MongoDB.
/// Provides event-driven replication for CQRS consistency.
/// </summary>
internal sealed class IdentityReadModelSyncHandler : 
    IDomainEventHandler<IdentityCreated>,
    IDomainEventHandler<IdentityModified>,
    IDomainEventHandler<IdentityRemoved>
{
    private readonly IReadModelSyncService _syncService;
    private readonly IIdentityRepository _identityRepository;
    private readonly ILogger<IdentityReadModelSyncHandler> _logger;

    public IdentityReadModelSyncHandler(
        IReadModelSyncService syncService,
        IIdentityRepository identityRepository,
        ILogger<IdentityReadModelSyncHandler> logger)
    {
        _syncService = syncService;
        _identityRepository = identityRepository;
        _logger = logger;
    }

    public async Task HandleAsync(IdentityCreated @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling IdentityCreated event for read model sync: IdentityId={IdentityId}",
            @event.IdentityId.Value);

        try
        {
            // Fetch the full identity from PostgreSQL
            var identity = await _identityRepository.GetAsync(@event.IdentityId, cancellationToken);
            if (identity == null)
            {
                _logger.LogWarning(
                    "Identity not found for read model sync: IdentityId={IdentityId}",
                    @event.IdentityId.Value);
                return;
            }

            // Sync to MongoDB read model
            await _syncService.SyncIdentityAsync(identity, cancellationToken);
            
            _logger.LogInformation(
                "Successfully synced IdentityCreated to MongoDB read model: IdentityId={IdentityId}",
                @event.IdentityId.Value);
        }
        catch (Exception ex)
        {
            // Log error but don't fail the event handler - sync failures are logged but don't block domain events
            _logger.LogError(ex,
                "Error syncing IdentityCreated to MongoDB read model: IdentityId={IdentityId}",
                @event.IdentityId.Value);
        }
    }

    public async Task HandleAsync(IdentityModified @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling IdentityModified event for read model sync: IdentityId={IdentityId}",
            @event.Identity.Id.Value);

        try
        {
            // The event contains the modified identity
            await _syncService.SyncIdentityAsync(@event.Identity, cancellationToken);
            
            _logger.LogInformation(
                "Successfully synced IdentityModified to MongoDB read model: IdentityId={IdentityId}",
                @event.Identity.Id.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error syncing IdentityModified to MongoDB read model: IdentityId={IdentityId}",
                @event.Identity.Id.Value);
        }
    }

    public async Task HandleAsync(IdentityRemoved @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling IdentityRemoved event for read model sync: IdentityId={IdentityId}",
            @event.Identity.Id.Value);

        try
        {
            // Remove from MongoDB read model
            await _syncService.RemoveIdentityAsync(@event.Identity.Id, cancellationToken);
            
            _logger.LogInformation(
                "Successfully synced IdentityRemoved to MongoDB read model: IdentityId={IdentityId}",
                @event.Identity.Id.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error syncing IdentityRemoved to MongoDB read model: IdentityId={IdentityId}",
                @event.Identity.Id.Value);
        }
    }
}
