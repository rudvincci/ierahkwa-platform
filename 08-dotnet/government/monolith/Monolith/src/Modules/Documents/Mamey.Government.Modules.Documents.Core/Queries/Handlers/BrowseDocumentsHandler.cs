using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Documents.Core.Domain.Repositories;
using Mamey.Government.Modules.Documents.Core.DTO;
using Mamey.Government.Modules.Documents.Core.Mappings;
using Mamey.MicroMonolith.Abstractions;
using Mamey.Types;

namespace Mamey.Government.Modules.Documents.Core.Queries.Handlers;

internal sealed class BrowseDocumentsHandler : IQueryHandler<BrowseDocuments, PagedResult<DocumentSummaryDto>>
{
    private readonly IDocumentRepository _repository;

    public BrowseDocumentsHandler(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<DocumentSummaryDto>> HandleAsync(BrowseDocuments query, CancellationToken cancellationToken = default)
    {
        var documents = await _repository.GetByTenantAsync(new TenantId(query.TenantId), cancellationToken);
        
        var filtered = documents.Where(d => d.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            filtered = filtered.Where(d => d.Category == query.Category);
        }

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            filtered = filtered.Where(d => 
                d.FileName.ToLower().Contains(term));
        }

        var list = filtered.ToList();
        var totalCount = list.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);
        var items = list
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(d => d.AsSummaryDto())
            .ToList();

        return PagedResult<DocumentSummaryDto>.Create(items, query.Page, query.PageSize, totalPages, totalCount);
    }
}
