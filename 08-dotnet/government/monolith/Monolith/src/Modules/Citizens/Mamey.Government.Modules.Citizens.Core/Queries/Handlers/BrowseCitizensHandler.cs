using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.DTO;
using Mamey.Types;

namespace Mamey.Government.Modules.Citizens.Core.Queries.Handlers;

internal sealed class BrowseCitizensHandler : IQueryHandler<BrowseCitizens, PagedResult<CitizenSummaryDto>>
{
    private readonly ICitizenRepository _repository;

    public BrowseCitizensHandler(ICitizenRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<CitizenSummaryDto>> HandleAsync(BrowseCitizens query, CancellationToken cancellationToken = default)
    {
        // Get citizens by tenant
        var citizens = await _repository.GetByTenantAsync(new TenantId(query.TenantId), cancellationToken);
        
        // Apply filters
        var filtered = citizens.AsEnumerable();
        
        if (!string.IsNullOrEmpty(query.Status) && Enum.TryParse<CitizenshipStatus>(query.Status, out var status))
        {
            filtered = filtered.Where(c => c.Status == status);
        }
        
        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            var search = query.SearchTerm.ToLowerInvariant();
            filtered = filtered.Where(c => 
                c.FirstName.ToLowerInvariant().Contains(search) ||
                c.LastName.ToLowerInvariant().Contains(search));
        }
        
        if (query.IsActive.HasValue)
        {
            filtered = query.IsActive.Value 
                ? filtered.Where(c => c.Status != CitizenshipStatus.Inactive)
                : filtered.Where(c => c.Status == CitizenshipStatus.Inactive);
        }

        // Apply pagination
        var filteredList = filtered.ToList();
        var totalCount = filteredList.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);
        var items = filteredList
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(c => new CitizenSummaryDto(
                c.Id.Value,
                c.FirstName,
                c.LastName,
                c.Status.ToString(),
                c.Status != CitizenshipStatus.Inactive))
            .ToList();

        return PagedResult<CitizenSummaryDto>.Create(
            items,
            query.Page,
            query.PageSize,
            totalPages,
            totalCount);
    }
}
