using Mamey.CQRS;
using Mamey.Government.Modules.Certificates.Core.Domain.Events;
using Mamey.Government.Modules.Certificates.Core.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Certificates.Core.Sync.EventHandlers;

internal sealed class CertificateReadModelSyncHandler : 
    IDomainEventHandler<CertificateIssued>,
    IDomainEventHandler<CertificateModified>,
    IDomainEventHandler<CertificateRevoked>
{
    private readonly IReadModelSyncService _syncService;
    private readonly ICertificateRepository _certificateRepository;
    private readonly ILogger<CertificateReadModelSyncHandler> _logger;

    public CertificateReadModelSyncHandler(
        IReadModelSyncService syncService,
        ICertificateRepository certificateRepository,
        ILogger<CertificateReadModelSyncHandler> logger)
    {
        _syncService = syncService;
        _certificateRepository = certificateRepository;
        _logger = logger;
    }

    public async Task HandleAsync(CertificateIssued @event, CancellationToken cancellationToken = default)
    {
        var certificate = await _certificateRepository.GetAsync(@event.CertificateId, cancellationToken);
        if (certificate != null)
        {
            await _syncService.SyncCertificateAsync(certificate, cancellationToken);
        }
    }

    public async Task HandleAsync(CertificateModified @event, CancellationToken cancellationToken = default)
    {
        await _syncService.SyncCertificateAsync(@event.Certificate, cancellationToken);
    }

    public async Task HandleAsync(CertificateRevoked @event, CancellationToken cancellationToken = default)
    {
        var certificate = await _certificateRepository.GetAsync(@event.CertificateId, cancellationToken);
        if (certificate != null)
        {
            await _syncService.SyncCertificateAsync(certificate, cancellationToken);
        }
    }
}
