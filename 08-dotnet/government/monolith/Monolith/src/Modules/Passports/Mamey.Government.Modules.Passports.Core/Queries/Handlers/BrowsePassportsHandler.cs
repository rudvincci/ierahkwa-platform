using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Passports.Core.Domain.Repositories;
using Mamey.Government.Modules.Passports.Core.DTO;
using Mamey.Government.Modules.Passports.Core.Mappings;
using Mamey.MicroMonolith.Abstractions;
using Mamey.Types;

namespace Mamey.Government.Modules.Passports.Core.Queries.Handlers;

internal sealed class BrowsePassportsHandler : IQueryHandler<BrowsePassports, PagedResult<PassportSummaryDto>>
{
    private readonly IPassportRepository _repository;

    public BrowsePassportsHandler(IPassportRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PassportSummaryDto>> HandleAsync(BrowsePassports query, CancellationToken cancellationToken = default)
    {
        var passports = await _repository.GetByTenantAsync(new TenantId(query.TenantId), cancellationToken);
        
        var filtered = passports.AsEnumerable();

        // Filter by status
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            filtered = query.Status.ToLower() switch
            {
                "active" => filtered.Where(p => p.IsActive && !p.IsExpired),
                "expired" => filtered.Where(p => p.IsExpired),
                "revoked" => filtered.Where(p => !p.IsActive),
                "expiringsoon" => filtered.Where(p => p.IsActive && p.ExpiryDate < DateTime.UtcNow.AddMonths(6) && !p.IsExpired),
                _ => filtered
            };
        }

        // Search by passport number
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            filtered = filtered.Where(p => 
                p.PassportNumber.Value.ToLower().Contains(term));
        }

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => p.AsSummaryDto())
            .ToList();

        return PagedResult<PassportSummaryDto>.Create(paged, query.Page, query.PageSize, total/query.PageSize,total);
    }
}
