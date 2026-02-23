using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Identity.Core.DTO;
using Mamey.MicroMonolith.Abstractions;

namespace Mamey.Government.Modules.Identity.Core.Queries;

internal class BrowseUserProfiles : IQuery<PagedResult<UserProfileSummaryDto>>
{
    public Guid? TenantId { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
