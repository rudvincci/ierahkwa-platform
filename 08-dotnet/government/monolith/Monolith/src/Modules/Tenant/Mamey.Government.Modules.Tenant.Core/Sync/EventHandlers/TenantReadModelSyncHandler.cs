using Mamey.CQRS;
using Mamey.Government.Modules.Tenant.Core.Domain.Events;
using Mamey.Government.Modules.Tenant.Core.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Tenant.Core.Sync.EventHandlers;

internal sealed class TenantReadModelSyncHandler : 
    IDomainEventHandler<TenantCreated>,
    IDomainEventHandler<TenantModified>,
    IDomainEventHandler<TenantRemoved>
{
    private readonly IReadModelSyncService _syncService;
    private readonly ITenantRepository _tenantRepository;
    private readonly ILogger<TenantReadModelSyncHandler> _logger;

    public TenantReadModelSyncHandler(
        IReadModelSyncService syncService,
        ITenantRepository tenantRepository,
        ILogger<TenantReadModelSyncHandler> logger)
    {
        _syncService = syncService;
        _tenantRepository = tenantRepository;
        _logger = logger;
    }

    public async Task HandleAsync(TenantCreated @event, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetAsync(@event.TenantId, cancellationToken);
        if (tenant != null)
        {
            await _syncService.SyncTenantAsync(tenant, cancellationToken);
        }
    }

    public async Task HandleAsync(TenantModified @event, CancellationToken cancellationToken = default)
    {
        await _syncService.SyncTenantAsync(@event.Tenant, cancellationToken);
    }

    public async Task HandleAsync(TenantRemoved @event, CancellationToken cancellationToken = default)
    {
        await _syncService.RemoveTenantAsync(@event.TenantId, cancellationToken);
    }
}
