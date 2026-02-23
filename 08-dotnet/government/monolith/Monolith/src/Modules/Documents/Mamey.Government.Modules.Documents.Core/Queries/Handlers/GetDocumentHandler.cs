using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Documents.Core.Domain.Repositories;
using Mamey.Government.Modules.Documents.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Documents.Core.DTO;
using Mamey.Government.Modules.Documents.Core.Mappings;

namespace Mamey.Government.Modules.Documents.Core.Queries.Handlers;

internal sealed class GetDocumentHandler : IQueryHandler<GetDocument, DocumentDto?>
{
    private readonly IDocumentRepository _repository;

    public GetDocumentHandler(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<DocumentDto?> HandleAsync(GetDocument query, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetAsync(new DocumentId(query.DocumentId), cancellationToken);
        return document?.AsDto();
    }
}
