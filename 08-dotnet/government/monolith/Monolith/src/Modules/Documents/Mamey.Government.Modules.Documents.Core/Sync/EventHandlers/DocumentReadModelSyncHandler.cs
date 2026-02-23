using Mamey.CQRS;
using Mamey.Government.Modules.Documents.Core.Domain.Events;
using Mamey.Government.Modules.Documents.Core.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Documents.Core.Sync.EventHandlers;

internal sealed class DocumentReadModelSyncHandler : 
    IDomainEventHandler<DocumentCreated>,
    IDomainEventHandler<DocumentModified>,
    IDomainEventHandler<DocumentDeleted>
{
    private readonly IReadModelSyncService _syncService;
    private readonly IDocumentRepository _documentRepository;
    private readonly ILogger<DocumentReadModelSyncHandler> _logger;

    public DocumentReadModelSyncHandler(
        IReadModelSyncService syncService,
        IDocumentRepository documentRepository,
        ILogger<DocumentReadModelSyncHandler> logger)
    {
        _syncService = syncService;
        _documentRepository = documentRepository;
        _logger = logger;
    }

    public async Task HandleAsync(DocumentCreated @event, CancellationToken cancellationToken = default)
    {
        var document = await _documentRepository.GetAsync(@event.DocumentId, cancellationToken);
        if (document != null)
        {
            await _syncService.SyncDocumentAsync(document, cancellationToken);
        }
    }

    public async Task HandleAsync(DocumentModified @event, CancellationToken cancellationToken = default)
    {
        await _syncService.SyncDocumentAsync(@event.Document, cancellationToken);
    }

    public async Task HandleAsync(DocumentDeleted @event, CancellationToken cancellationToken = default)
    {
        await _syncService.RemoveDocumentAsync(@event.DocumentId, cancellationToken);
    }
}
