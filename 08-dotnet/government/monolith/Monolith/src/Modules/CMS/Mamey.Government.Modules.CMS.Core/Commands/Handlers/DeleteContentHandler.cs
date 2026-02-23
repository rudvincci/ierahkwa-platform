using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.CMS.Core.Domain.Repositories;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.CMS.Core.Commands.Handlers;

internal sealed class DeleteContentHandler : ICommandHandler<DeleteContent>
{
    private readonly IContentRepository _repository;
    private readonly ILogger<DeleteContentHandler> _logger;

    public DeleteContentHandler(
        IContentRepository repository,
        ILogger<DeleteContentHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task HandleAsync(DeleteContent command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting content: {ContentId}", command.ContentId);

        var content = await _repository.GetAsync(new ContentId(command.ContentId), cancellationToken);
        if (content is null)
        {
            throw new InvalidOperationException($"Content with ID {command.ContentId} not found");
        }

        content.Archive();

        await _repository.UpdateAsync(content, cancellationToken);

        _logger.LogInformation("Content deleted (archived): {ContentId}", command.ContentId);
    }
}
