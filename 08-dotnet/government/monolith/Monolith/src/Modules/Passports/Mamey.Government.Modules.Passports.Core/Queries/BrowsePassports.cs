using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Passports.Core.DTO;
using Mamey.MicroMonolith.Abstractions;

namespace Mamey.Government.Modules.Passports.Core.Queries;

internal class BrowsePassports : IQuery<PagedResult<PassportSummaryDto>>
{
    public Guid TenantId { get; set; }
    public string? Status { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
