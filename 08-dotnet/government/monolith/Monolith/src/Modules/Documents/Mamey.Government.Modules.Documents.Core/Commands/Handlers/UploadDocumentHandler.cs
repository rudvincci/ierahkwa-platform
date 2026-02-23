using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Documents.Core.Domain.Entities;
using Mamey.Government.Modules.Documents.Core.Domain.Repositories;
using Mamey.Government.Modules.Documents.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Documents.Core.Events;
using Mamey.MicroMonolith.Abstractions.Messaging;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Documents.Core.Commands.Handlers;

internal sealed class UploadDocumentHandler : ICommandHandler<UploadDocument>
{
    private readonly IDocumentRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<UploadDocumentHandler> _logger;

    public UploadDocumentHandler(
        IDocumentRepository repository,
        IMessageBroker messageBroker,
        ILogger<UploadDocumentHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(UploadDocument command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Uploading document: {FileName}", command.FileName);

        var document = new Document(
            new DocumentId(command.Id),
            new TenantId(command.TenantId),
            command.FileName,
            command.ContentType,
            command.FileSize,
            command.StorageBucket,
            command.StorageKey,
            command.Category);

        if (!string.IsNullOrWhiteSpace(command.Description))
        {
            document.UpdateDescription(command.Description);
        }

        await _repository.AddAsync(document, cancellationToken);

        await _messageBroker.PublishAsync(
            new DocumentUploadedEvent(document.Id.Value, command.TenantId, command.FileName),
            cancellationToken);

        _logger.LogInformation("Document uploaded: {DocumentId} - {FileName}", 
            command.Id, command.FileName);
    }
}
