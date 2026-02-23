using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Documents.Core.DTO;
using Mamey.MicroMonolith.Abstractions;

namespace Mamey.Government.Modules.Documents.Core.Queries;

internal class BrowseDocuments : IQuery<PagedResult<DocumentSummaryDto>>
{
    public Guid TenantId { get; set; }
    public string? Category { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
