using System;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Certificates.Core.DTO;
using Mamey.MicroMonolith.Abstractions;

namespace Mamey.Government.Modules.Certificates.Core.Queries;

internal class BrowseCertificates : IQuery<PagedResult<CertificateSummaryDto>>
{
    public Guid TenantId { get; set; }
    public string? CertificateType { get; set; }
    public string? Status { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
