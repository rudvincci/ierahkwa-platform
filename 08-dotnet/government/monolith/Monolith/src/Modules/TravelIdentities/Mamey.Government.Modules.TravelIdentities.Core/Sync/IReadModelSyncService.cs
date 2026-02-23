using Mamey.Government.Modules.TravelIdentities.Core.Domain.Entities;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.TravelIdentities.Core.Sync;

internal interface IReadModelSyncService
{
    Task SyncTravelIdentityAsync(TravelIdentity travelIdentity, CancellationToken cancellationToken = default);
    Task RemoveTravelIdentityAsync(TravelIdentityId travelIdentityId, CancellationToken cancellationToken = default);
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
