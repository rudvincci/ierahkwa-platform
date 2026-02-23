using Mamey.CQRS;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Events;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.TravelIdentities.Core.Sync.EventHandlers;

internal sealed class TravelIdentityReadModelSyncHandler : 
    IDomainEventHandler<TravelIdentityIssued>,
    IDomainEventHandler<TravelIdentityModified>,
    IDomainEventHandler<TravelIdentityRenewed>,
    IDomainEventHandler<TravelIdentityRevoked>
{
    private readonly IReadModelSyncService _syncService;
    private readonly ITravelIdentityRepository _travelIdentityRepository;
    private readonly ILogger<TravelIdentityReadModelSyncHandler> _logger;

    public TravelIdentityReadModelSyncHandler(
        IReadModelSyncService syncService,
        ITravelIdentityRepository travelIdentityRepository,
        ILogger<TravelIdentityReadModelSyncHandler> logger)
    {
        _syncService = syncService;
        _travelIdentityRepository = travelIdentityRepository;
        _logger = logger;
    }

    public async Task HandleAsync(TravelIdentityIssued @event, CancellationToken cancellationToken = default)
    {
        var travelIdentity = await _travelIdentityRepository.GetAsync(@event.TravelIdentityId, cancellationToken);
        if (travelIdentity != null)
        {
            await _syncService.SyncTravelIdentityAsync(travelIdentity, cancellationToken);
        }
    }

    public async Task HandleAsync(TravelIdentityModified @event, CancellationToken cancellationToken = default)
    {
        await _syncService.SyncTravelIdentityAsync(@event.TravelIdentity, cancellationToken);
    }

    public async Task HandleAsync(TravelIdentityRenewed @event, CancellationToken cancellationToken = default)
    {
        var travelIdentity = await _travelIdentityRepository.GetAsync(@event.TravelIdentityId, cancellationToken);
        if (travelIdentity != null)
        {
            await _syncService.SyncTravelIdentityAsync(travelIdentity, cancellationToken);
        }
    }

    public async Task HandleAsync(TravelIdentityRevoked @event, CancellationToken cancellationToken = default)
    {
        var travelIdentity = await _travelIdentityRepository.GetAsync(@event.TravelIdentityId, cancellationToken);
        if (travelIdentity != null)
        {
            await _syncService.SyncTravelIdentityAsync(travelIdentity, cancellationToken);
        }
    }
}
