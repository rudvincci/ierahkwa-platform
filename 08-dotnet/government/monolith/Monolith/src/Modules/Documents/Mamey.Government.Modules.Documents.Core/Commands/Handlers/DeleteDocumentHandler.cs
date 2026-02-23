using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Documents.Core.Domain.Repositories;
using Mamey.Government.Modules.Documents.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Documents.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Documents.Core.Commands.Handlers;

internal sealed class DeleteDocumentHandler : ICommandHandler<DeleteDocument>
{
    private readonly IDocumentRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<DeleteDocumentHandler> _logger;

    public DeleteDocumentHandler(
        IDocumentRepository repository,
        IMessageBroker messageBroker,
        ILogger<DeleteDocumentHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(DeleteDocument command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting document: {DocumentId}", command.DocumentId);

        var document = await _repository.GetAsync(new DocumentId(command.DocumentId), cancellationToken);
        if (document is null)
        {
            throw new InvalidOperationException($"Document with ID {command.DocumentId} not found");
        }

        document.Delete();

        await _repository.UpdateAsync(document, cancellationToken);

        await _messageBroker.PublishAsync(
            new DocumentDeletedEvent(document.Id.Value, document.TenantId.Value),
            cancellationToken);

        _logger.LogInformation("Document deleted: {DocumentId}", command.DocumentId);
    }
}
