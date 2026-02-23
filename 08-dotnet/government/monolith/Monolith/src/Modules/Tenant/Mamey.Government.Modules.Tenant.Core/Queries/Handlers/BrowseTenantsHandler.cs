using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Tenant.Core.Domain.Repositories;
using Mamey.Government.Modules.Tenant.Core.DTO;
using Mamey.Government.Modules.Tenant.Core.Mappings;
using Mamey.MicroMonolith.Abstractions;

namespace Mamey.Government.Modules.Tenant.Core.Queries.Handlers;

internal sealed class BrowseTenantsHandler : IQueryHandler<BrowseTenants, PagedResult<TenantSummaryDto>>
{
    private readonly ITenantRepository _repository;

    public BrowseTenantsHandler(ITenantRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<TenantSummaryDto>> HandleAsync(BrowseTenants query, CancellationToken cancellationToken = default)
    {
        var tenants = await _repository.BrowseAsync(cancellationToken);
        
        var filtered = tenants.AsEnumerable();

        // Filter by status
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            filtered = query.Status.ToLower() switch
            {
                "active" => filtered.Where(t => t.IsActive),
                "suspended" => filtered.Where(t => !t.IsActive),
                _ => filtered
            };
        }

        // Search by name or domain
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            filtered = filtered.Where(t => 
                t.DisplayName.ToLower().Contains(term) ||
                (t.Domain?.ToLower().Contains(term) ?? false));
        }

        var list = filtered.ToList();
        var totalCount = list.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);
        var items = list
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(t => t.AsSummaryDto())
            .ToList();

        return PagedResult<TenantSummaryDto>.Create(items, query.Page, query.PageSize, totalPages, totalCount);
    }
}
