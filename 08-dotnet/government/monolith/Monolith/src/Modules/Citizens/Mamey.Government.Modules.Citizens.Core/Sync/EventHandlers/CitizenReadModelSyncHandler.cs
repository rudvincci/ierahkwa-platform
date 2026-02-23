using Mamey.CQRS;
using Mamey.Government.Modules.Citizens.Core.Domain.Events;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Citizens.Core.Sync.EventHandlers;

internal sealed class CitizenReadModelSyncHandler : 
    IDomainEventHandler<CitizenCreated>,
    IDomainEventHandler<CitizenModified>,
    IDomainEventHandler<CitizenStatusChanged>,
    IDomainEventHandler<CitizenDeactivated>
{
    private readonly IReadModelSyncService _syncService;
    private readonly ICitizenRepository _citizenRepository;
    private readonly ILogger<CitizenReadModelSyncHandler> _logger;

    public CitizenReadModelSyncHandler(
        IReadModelSyncService syncService,
        ICitizenRepository citizenRepository,
        ILogger<CitizenReadModelSyncHandler> logger)
    {
        _syncService = syncService;
        _citizenRepository = citizenRepository;
        _logger = logger;
    }

    public async Task HandleAsync(CitizenCreated @event, CancellationToken cancellationToken = default)
    {
        var citizen = await _citizenRepository.GetAsync(@event.CitizenId, cancellationToken);
        if (citizen != null)
        {
            await _syncService.SyncCitizenAsync(citizen, cancellationToken);
        }
    }

    public async Task HandleAsync(CitizenModified @event, CancellationToken cancellationToken = default)
    {
        await _syncService.SyncCitizenAsync(@event.Citizen, cancellationToken);
    }

    public async Task HandleAsync(CitizenStatusChanged @event, CancellationToken cancellationToken = default)
    {
        var citizen = await _citizenRepository.GetAsync(@event.CitizenId, cancellationToken);
        if (citizen != null)
        {
            await _syncService.SyncCitizenAsync(citizen, cancellationToken);
        }
    }

    public async Task HandleAsync(CitizenDeactivated @event, CancellationToken cancellationToken = default)
    {
        var citizen = await _citizenRepository.GetAsync(@event.CitizenId, cancellationToken);
        if (citizen != null)
        {
            await _syncService.SyncCitizenAsync(citizen, cancellationToken);
        }
    }
}
