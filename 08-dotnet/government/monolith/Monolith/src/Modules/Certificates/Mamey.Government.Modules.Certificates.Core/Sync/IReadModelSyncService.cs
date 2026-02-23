using Mamey.Government.Modules.Certificates.Core.Domain.Entities;
using Mamey.Government.Modules.Certificates.Core.Domain.ValueObjects;

namespace Mamey.Government.Modules.Certificates.Core.Sync;

internal interface IReadModelSyncService
{
    Task SyncCertificateAsync(Certificate certificate, CancellationToken cancellationToken = default);
    Task RemoveCertificateAsync(CertificateId certificateId, CancellationToken cancellationToken = default);
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
