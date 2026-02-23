using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.CMS.Core.Domain.Repositories;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.CMS.Core.Commands.Handlers;

internal sealed class UnpublishContentHandler : ICommandHandler<UnpublishContent>
{
    private readonly IContentRepository _repository;
    private readonly ILogger<UnpublishContentHandler> _logger;

    public UnpublishContentHandler(
        IContentRepository repository,
        ILogger<UnpublishContentHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task HandleAsync(UnpublishContent command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Unpublishing content: {ContentId}", command.ContentId);

        var content = await _repository.GetAsync(new ContentId(command.ContentId), cancellationToken);
        if (content is null)
        {
            throw new InvalidOperationException($"Content with ID {command.ContentId} not found");
        }

        content.UpdateStatus(ContentStatus.Draft);

        await _repository.UpdateAsync(content, cancellationToken);

        _logger.LogInformation("Content unpublished: {ContentId}", command.ContentId);
    }
}
