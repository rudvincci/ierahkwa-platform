using Mamey.Government.Modules.Notifications.Core.Domain.Entities;
using Mamey.Government.Modules.Notifications.Core.Domain.Types;

namespace Mamey.Government.Modules.Notifications.Core.Sync;

/// <summary>
/// Service for synchronizing write model (PostgreSQL) to read model (MongoDB) in real-time.
/// Provides event-driven replication for CQRS consistency.
/// </summary>
internal interface IReadModelSyncService
{
    /// <summary>
    /// Syncs a notification to MongoDB read model.
    /// </summary>
    Task SyncNotificationAsync(Notification notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a notification from MongoDB read model.
    /// </summary>
    Task RemoveNotificationAsync(NotificationId notificationId, CancellationToken cancellationToken = default);

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
