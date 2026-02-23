using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.CMS.Core.DTO;
using Mamey.MicroMonolith.Abstractions;

namespace Mamey.Government.Modules.CMS.Core.Queries;

internal class BrowseContent : IQuery<PagedResult<ContentSummaryDto>>
{
    public Guid TenantId { get; set; }
    public string? ContentType { get; set; }
    public string? Status { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
