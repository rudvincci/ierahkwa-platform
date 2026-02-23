using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.DTO;
using Mamey.MicroMonolith.Abstractions;
using Mamey.Types;

namespace Mamey.Government.Modules.Citizens.Core.Queries.Handlers;

internal sealed class GetCitizensByStatusHandler : IQueryHandler<GetCitizensByStatus, PagedResult<CitizenSummaryDto>>
{
    private readonly ICitizenRepository _repository;

    public GetCitizensByStatusHandler(ICitizenRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<CitizenSummaryDto>> HandleAsync(GetCitizensByStatus query, CancellationToken cancellationToken = default)
    {
        var citizens = await _repository.GetByTenantAsync(new TenantId(query.TenantId), cancellationToken);
        
        var filtered = citizens.Where(c => c.Status == query.Status).ToList();
        
        var totalCount = filtered.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);
        var items = filtered
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new CitizenSummaryDto(
                c.Id.Value,
                c.FirstName,
                c.LastName,
                c.Status.ToString(),
                c.Status != CitizenshipStatus.Inactive))
            .ToList();

        return PagedResult<CitizenSummaryDto>.Create(items, query.Page, query.PageSize, totalPages, totalCount);
    }
}
