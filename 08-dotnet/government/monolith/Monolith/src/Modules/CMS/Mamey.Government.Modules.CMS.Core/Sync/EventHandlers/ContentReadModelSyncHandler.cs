using Mamey.CQRS;
using Mamey.Government.Modules.CMS.Core.Domain.Events;
using Mamey.Government.Modules.CMS.Core.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.CMS.Core.Sync.EventHandlers;

internal sealed class ContentReadModelSyncHandler : 
    IDomainEventHandler<ContentCreated>,
    IDomainEventHandler<ContentModified>,
    IDomainEventHandler<ContentPublished>
{
    private readonly IReadModelSyncService _syncService;
    private readonly IContentRepository _contentRepository;
    private readonly ILogger<ContentReadModelSyncHandler> _logger;

    public ContentReadModelSyncHandler(
        IReadModelSyncService syncService,
        IContentRepository contentRepository,
        ILogger<ContentReadModelSyncHandler> logger)
    {
        _syncService = syncService;
        _contentRepository = contentRepository;
        _logger = logger;
    }

    public async Task HandleAsync(ContentCreated @event, CancellationToken cancellationToken = default)
    {
        var content = await _contentRepository.GetAsync(@event.ContentId, cancellationToken);
        if (content != null)
        {
            await _syncService.SyncContentAsync(content, cancellationToken);
        }
    }

    public async Task HandleAsync(ContentModified @event, CancellationToken cancellationToken = default)
    {
        await _syncService.SyncContentAsync(@event.Content, cancellationToken);
    }

    public async Task HandleAsync(ContentPublished @event, CancellationToken cancellationToken = default)
    {
        var content = await _contentRepository.GetAsync(@event.ContentId, cancellationToken);
        if (content != null)
        {
            await _syncService.SyncContentAsync(content, cancellationToken);
        }
    }
}
