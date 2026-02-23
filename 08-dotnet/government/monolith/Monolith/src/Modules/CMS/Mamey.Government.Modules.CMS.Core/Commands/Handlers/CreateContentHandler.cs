using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.CMS.Core.Domain.Entities;
using Mamey.Government.Modules.CMS.Core.Domain.Repositories;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CMS.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.CMS.Core.Commands.Handlers;

internal sealed class CreateContentHandler : ICommandHandler<CreateContent>
{
    private readonly IContentRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<CreateContentHandler> _logger;

    public CreateContentHandler(
        IContentRepository repository,
        IMessageBroker messageBroker,
        ILogger<CreateContentHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(CreateContent command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating content: {Title}", command.Title);

        var content = new Content(
            new ContentId(command.Id),
            new TenantId(command.TenantId),
            command.Title,
            command.Slug,
            command.ContentType);

        if (!string.IsNullOrWhiteSpace(command.Body))
        {
            content.UpdateContent(command.Body, command.Excerpt);
        }

        await _repository.AddAsync(content, cancellationToken);

        await _messageBroker.PublishAsync(
            new ContentCreatedEvent(content.Id.Value, command.TenantId, command.Title, command.Slug),
            cancellationToken);

        _logger.LogInformation("Content created: {ContentId} - {Title}", command.Id, command.Title);
    }
}
