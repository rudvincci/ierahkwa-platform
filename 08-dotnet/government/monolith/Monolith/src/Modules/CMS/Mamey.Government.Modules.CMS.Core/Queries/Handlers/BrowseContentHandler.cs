using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CMS.Core.Domain.Repositories;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;
using Mamey.Government.Modules.CMS.Core.DTO;
using Mamey.Government.Modules.CMS.Core.Mappings;
using Mamey.MicroMonolith.Abstractions;
using Mamey.Types;

namespace Mamey.Government.Modules.CMS.Core.Queries.Handlers;

internal sealed class BrowseContentHandler : IQueryHandler<BrowseContent, PagedResult<ContentSummaryDto>>
{
    private readonly IContentRepository _repository;

    public BrowseContentHandler(IContentRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ContentSummaryDto>> HandleAsync(BrowseContent query, CancellationToken cancellationToken = default)
    {
        var contents = await _repository.GetByTenantAsync(new TenantId(query.TenantId), cancellationToken);
        
        var filtered = contents.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query.ContentType))
        {
            filtered = filtered.Where(c => c.ContentType.Equals(query.ContentType, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query.Status) && 
            Enum.TryParse<ContentStatus>(query.Status, true, out var status))
        {
            filtered = filtered.Where(c => c.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            filtered = filtered.Where(c => 
                c.Title.ToLower().Contains(term) ||
                c.Slug.ToLower().Contains(term));
        }

        var list = filtered.ToList();
        var totalCount = list.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);
        var items = list
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => c.AsSummaryDto())
            .ToList();

        return PagedResult<ContentSummaryDto>.Create(items, query.Page, query.PageSize, totalPages, totalCount);
    }
}
