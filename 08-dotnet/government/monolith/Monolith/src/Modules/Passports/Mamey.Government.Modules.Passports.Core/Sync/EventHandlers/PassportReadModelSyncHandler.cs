using Mamey.CQRS;
using Mamey.Government.Modules.Passports.Core.Domain.Events;
using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Passports.Core.Sync.EventHandlers;

internal sealed class PassportReadModelSyncHandler : 
    IDomainEventHandler<PassportIssued>,
    IDomainEventHandler<PassportModified>,
    IDomainEventHandler<PassportRenewed>,
    IDomainEventHandler<PassportRevoked>
{
    private readonly IReadModelSyncService _syncService;
    private readonly IPassportRepository _passportRepository;
    private readonly ILogger<PassportReadModelSyncHandler> _logger;

    public PassportReadModelSyncHandler(
        IReadModelSyncService syncService,
        IPassportRepository passportRepository,
        ILogger<PassportReadModelSyncHandler> logger)
    {
        _syncService = syncService;
        _passportRepository = passportRepository;
        _logger = logger;
    }

    public async Task HandleAsync(PassportIssued @event, CancellationToken cancellationToken = default)
    {
        var passport = await _passportRepository.GetAsync(@event.PassportId, cancellationToken);
        if (passport != null)
        {
            await _syncService.SyncPassportAsync(passport, cancellationToken);
        }
    }

    public async Task HandleAsync(PassportModified @event, CancellationToken cancellationToken = default)
    {
        await _syncService.SyncPassportAsync(@event.Passport, cancellationToken);
    }

    public async Task HandleAsync(PassportRenewed @event, CancellationToken cancellationToken = default)
    {
        var passport = await _passportRepository.GetAsync(@event.PassportId, cancellationToken);
        if (passport != null)
        {
            await _syncService.SyncPassportAsync(passport, cancellationToken);
        }
    }

    public async Task HandleAsync(PassportRevoked @event, CancellationToken cancellationToken = default)
    {
        var passport = await _passportRepository.GetAsync(@event.PassportId, cancellationToken);
        if (passport != null)
        {
            await _syncService.SyncPassportAsync(passport, cancellationToken);
        }
    }
}
