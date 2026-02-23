using Mamey.Government.Modules.Tenant.Core.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Modules.Tenant.Core.Sync;

internal interface IReadModelSyncService
{
    Task SyncTenantAsync(TenantEntity tenant, CancellationToken cancellationToken = default);
    Task RemoveTenantAsync(TenantId tenantId, CancellationToken cancellationToken = default);
    Task<SyncStatus> GetSyncStatusAsync(CancellationToken cancellationToken = default);
}

public class SyncStatus
{
    public DateTime LastSyncTime { get; set; }
    public long TotalSynced { get; set; }
    public long TotalFailed { get; set; }
    public TimeSpan AverageSyncLatency { get; set; }
    public bool IsHealthy { get; set; }
}
