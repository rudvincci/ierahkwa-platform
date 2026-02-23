using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.CMS.Core.Domain.Repositories;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CMS.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.CMS.Core.Commands.Handlers;

internal sealed class PublishContentHandler : ICommandHandler<PublishContent>
{
    private readonly IContentRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<PublishContentHandler> _logger;

    public PublishContentHandler(
        IContentRepository repository,
        IMessageBroker messageBroker,
        ILogger<PublishContentHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(PublishContent command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing content: {ContentId}", command.ContentId);

        var content = await _repository.GetAsync(new ContentId(command.ContentId), cancellationToken);
        if (content is null)
        {
            throw new InvalidOperationException($"Content with ID {command.ContentId} not found");
        }

        content.Publish();

        await _repository.UpdateAsync(content, cancellationToken);

        await _messageBroker.PublishAsync(
            new ContentPublishedEvent(content.Id.Value, content.PublishedAt!.Value),
            cancellationToken);

        _logger.LogInformation("Content published: {ContentId}", command.ContentId);
    }
}
