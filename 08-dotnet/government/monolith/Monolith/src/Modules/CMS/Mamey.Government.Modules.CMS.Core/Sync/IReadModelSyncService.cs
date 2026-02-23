using Mamey.Government.Modules.CMS.Core.Domain.Entities;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.CMS.Core.Sync;

internal interface IReadModelSyncService
{
    Task SyncContentAsync(Content content, CancellationToken cancellationToken = default);
    Task RemoveContentAsync(ContentId contentId, CancellationToken cancellationToken = default);
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
