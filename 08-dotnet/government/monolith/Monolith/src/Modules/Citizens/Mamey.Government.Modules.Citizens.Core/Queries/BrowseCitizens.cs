using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Citizens.Core.DTO;

namespace Mamey.Government.Modules.Citizens.Core.Queries;

public record BrowseCitizens(
    Guid TenantId,
    string? Status = null,
    string? SearchTerm = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<CitizenSummaryDto>>;
