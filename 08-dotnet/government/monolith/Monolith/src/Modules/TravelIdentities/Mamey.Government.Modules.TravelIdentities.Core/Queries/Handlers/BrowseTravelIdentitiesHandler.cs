using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Government.Modules.TravelIdentities.Core.DTO;
using Mamey.Government.Modules.TravelIdentities.Core.Mappings;
using Mamey.MicroMonolith.Abstractions;
using Mamey.Types;

namespace Mamey.Government.Modules.TravelIdentities.Core.Queries.Handlers;

internal sealed class BrowseTravelIdentitiesHandler : IQueryHandler<BrowseTravelIdentities, PagedResult<TravelIdentitySummaryDto>>
{
    private readonly ITravelIdentityRepository _repository;

    public BrowseTravelIdentitiesHandler(ITravelIdentityRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<TravelIdentitySummaryDto>> HandleAsync(BrowseTravelIdentities query, CancellationToken cancellationToken = default)
    {
        var travelIdentities = await _repository.GetByTenantAsync(new TenantId(query.TenantId), cancellationToken);
        
        var filtered = travelIdentities.AsEnumerable();

        // Filter by status
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            filtered = query.Status.ToLower() switch
            {
                "active" => filtered.Where(t => t.IsActive && !t.IsExpired),
                "expired" => filtered.Where(t => t.IsExpired),
                "revoked" => filtered.Where(t => !t.IsActive),
                "expiringsoon" => filtered.Where(t => t.IsActive && t.ExpiryDate < DateTime.UtcNow.AddMonths(6) && !t.IsExpired),
                _ => filtered
            };
        }

        // Search by ID number
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            filtered = filtered.Where(t => 
                t.TravelIdentityNumber.Value.ToLower().Contains(term));
        }

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(t => t.AsSummaryDto())
            .ToList();

        return  PagedResult<TravelIdentitySummaryDto>.Create(paged, query.Page, query.PageSize, total/query.PageSize, total);
    }
}
