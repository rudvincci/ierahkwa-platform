using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Modules.Citizens.Core.DTO;
using Mamey.MicroMonolith.Abstractions;

namespace Mamey.Government.Modules.Citizens.Core.Queries;

internal class GetCitizensByStatus : IQuery<PagedResult<CitizenSummaryDto>>
{
    public Guid TenantId { get; set; }
    public CitizenshipStatus Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
