using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Documents.Core.Domain.Repositories;
using Mamey.Government.Modules.Documents.Core.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Documents.Core.Commands.Handlers;

internal sealed class UpdateDocumentMetadataHandler : ICommandHandler<UpdateDocumentMetadata>
{
    private readonly IDocumentRepository _repository;
    private readonly ILogger<UpdateDocumentMetadataHandler> _logger;

    public UpdateDocumentMetadataHandler(
        IDocumentRepository repository,
        ILogger<UpdateDocumentMetadataHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task HandleAsync(UpdateDocumentMetadata command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating document metadata: {DocumentId}", command.DocumentId);

        var document = await _repository.GetAsync(new DocumentId(command.DocumentId), cancellationToken);
        if (document is null)
        {
            throw new InvalidOperationException($"Document with ID {command.DocumentId} not found");
        }

        if (command.Description is not null)
        {
            document.UpdateDescription(command.Description);
        }

        if (command.Tags is not null)
        {
            foreach (var tag in command.Tags)
            {
                document.UpdateMetadata(tag.Key, tag.Value);
            }
        }

        await _repository.UpdateAsync(document, cancellationToken);

        _logger.LogInformation("Document metadata updated: {DocumentId}", command.DocumentId);
    }
}
