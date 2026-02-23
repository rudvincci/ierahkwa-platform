using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.CMS.Core.Domain.Repositories;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CMS.Core.Exceptions;

namespace Mamey.Government.Modules.CMS.Core.Commands.Handlers;

internal sealed class UpdateContentHandler : ICommandHandler<UpdateContent>
{
    private readonly IContentRepository _repository;

    public UpdateContentHandler(IContentRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(UpdateContent command, CancellationToken cancellationToken = default)
    {
        var content = await _repository.GetAsync(new ContentId(command.ContentId), cancellationToken);
        if (content is null)
        {
            throw new ContentNotFoundException(command.ContentId);
        }

        content.UpdateContent(command.Body ?? content.Body, command.Excerpt ?? content.Excerpt);
        await _repository.UpdateAsync(content, cancellationToken);
    }
}
