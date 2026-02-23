using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Queries;

public record BrowseApplications(
    Guid TenantId,
    string? Status = null,
    string? SearchTerm = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<ApplicationSummaryDto>>;
