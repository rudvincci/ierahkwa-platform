using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CMS.Core.Domain.Repositories;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CMS.Core.DTO;
using Mamey.Government.Modules.CMS.Core.Mappings;

namespace Mamey.Government.Modules.CMS.Core.Queries.Handlers;

internal sealed class GetContentHandler : IQueryHandler<GetContent, ContentDto?>
{
    private readonly IContentRepository _repository;

    public GetContentHandler(IContentRepository repository)
    {
        _repository = repository;
    }

    public async Task<ContentDto?> HandleAsync(GetContent query, CancellationToken cancellationToken = default)
    {
        var content = await _repository.GetAsync(new ContentId(query.ContentId), cancellationToken);
        return content?.AsDto();
    }
}
