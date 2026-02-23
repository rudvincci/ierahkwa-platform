using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Infrastructure.Sync;

/// <summary>
/// Service for synchronizing write model (PostgreSQL) to read model (MongoDB) in real-time.
/// Provides event-driven replication for CQRS consistency.
/// </summary>
internal interface IReadModelSyncService
{
    /// <summary>
    /// Syncs an identity to MongoDB read model.
    /// </summary>
    Task SyncIdentityAsync(Identity identity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an identity from MongoDB read model.
    /// </summary>
    Task RemoveIdentityAsync(IdentityId identityId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sync status and metrics.
    /// </summary>
    Task<SyncStatus> GetSyncStatusAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Sync status and metrics.
/// </summary>
public class SyncStatus
{
    public DateTime LastSyncTime { get; set; }
    public long TotalSynced { get; set; }
    public long TotalFailed { get; set; }
    public TimeSpan AverageSyncLatency { get; set; }
    public bool IsHealthy { get; set; }
}
