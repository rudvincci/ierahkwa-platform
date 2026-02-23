using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CMS.Core.Domain.Repositories;
using Mamey.Government.Modules.CMS.Core.DTO;
using Mamey.Government.Modules.CMS.Core.Mappings;

namespace Mamey.Government.Modules.CMS.Core.Queries.Handlers;

internal sealed class GetContentBySlugHandler : IQueryHandler<GetContentBySlug, ContentDto?>
{
    private readonly IContentRepository _repository;

    public GetContentBySlugHandler(IContentRepository repository)
    {
        _repository = repository;
    }

    public async Task<ContentDto?> HandleAsync(GetContentBySlug query, CancellationToken cancellationToken = default)
    {
        var content = await _repository.GetBySlugAsync(query.Slug, cancellationToken);
        return content?.AsDto();
    }
}
