using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Identity.Core.Domain.Repositories;
using Mamey.Government.Modules.Identity.Core.DTO;
using Mamey.Government.Modules.Identity.Core.Mappings;
using Mamey.MicroMonolith.Abstractions;

namespace Mamey.Government.Modules.Identity.Core.Queries.Handlers;

internal sealed class BrowseUserProfilesHandler : IQueryHandler<BrowseUserProfiles, PagedResult<UserProfileSummaryDto>>
{
    private readonly IUserProfileRepository _repository;

    public BrowseUserProfilesHandler(IUserProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<UserProfileSummaryDto>> HandleAsync(BrowseUserProfiles query, CancellationToken cancellationToken = default)
    {
        var profiles = await _repository.BrowseAsync(cancellationToken);
        
        var filtered = profiles.AsEnumerable();

        if (query.TenantId.HasValue)
        {
            filtered = filtered.Where(p => p.TenantId == query.TenantId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            filtered = filtered.Where(p => 
                (p.Email?.ToLower().Contains(term) ?? false) ||
                (p.DisplayName?.ToLower().Contains(term) ?? false));
        }

        var list = filtered.ToList();
        var totalCount = list.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);
        var items = list
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => p.AsSummaryDto())
            .ToList();

        return PagedResult<UserProfileSummaryDto>.Create(items, query.Page, query.PageSize, totalPages, totalCount);
    }
}
