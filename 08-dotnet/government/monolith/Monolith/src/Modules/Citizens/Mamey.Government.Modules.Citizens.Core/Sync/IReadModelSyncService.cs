using Mamey.Government.Modules.Citizens.Core.Domain.Entities;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.Citizens.Core.Sync;

internal interface IReadModelSyncService
{
    Task SyncCitizenAsync(Citizen citizen, CancellationToken cancellationToken = default);
    Task RemoveCitizenAsync(CitizenId citizenId, CancellationToken cancellationToken = default);
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
