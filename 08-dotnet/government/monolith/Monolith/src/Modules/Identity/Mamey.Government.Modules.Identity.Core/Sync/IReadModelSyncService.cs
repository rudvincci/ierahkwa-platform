using Mamey.Government.Modules.Identity.Core.Domain.Entities;
using Mamey.Types;

namespace Mamey.Government.Modules.Identity.Core.Sync;

/// <summary>
/// Service for synchronizing write model (PostgreSQL) to read model (MongoDB) in real-time.
/// Provides event-driven replication for CQRS consistency.
/// </summary>
internal interface IReadModelSyncService
{
    /// <summary>
    /// Syncs a user profile to MongoDB read model.
    /// </summary>
    Task SyncUserProfileAsync(UserProfile userProfile, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a user profile from MongoDB read model.
    /// </summary>
    Task RemoveUserProfileAsync(UserId userId, CancellationToken cancellationToken = default);

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
