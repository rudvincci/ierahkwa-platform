using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Tenant.Core.DTO;
using Mamey.MicroMonolith.Abstractions;

namespace Mamey.Government.Modules.Tenant.Core.Queries;

internal class BrowseTenants : IQuery<PagedResult<TenantSummaryDto>>
{
    public string? Status { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
