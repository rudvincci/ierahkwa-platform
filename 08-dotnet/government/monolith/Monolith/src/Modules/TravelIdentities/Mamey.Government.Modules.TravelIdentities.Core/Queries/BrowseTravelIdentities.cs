using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.TravelIdentities.Core.DTO;
using Mamey.MicroMonolith.Abstractions;

namespace Mamey.Government.Modules.TravelIdentities.Core.Queries;

internal class BrowseTravelIdentities : IQuery<PagedResult<TravelIdentitySummaryDto>>
{
    public Guid TenantId { get; set; }
    public string? Status { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
