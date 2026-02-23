using System;
using Mamey.CQRS.Queries;
using Pupitre.Compliance.Application.DTO;


namespace Pupitre.Compliance.Application.Queries;

internal class BrowseCompliance : PagedQueryBase, IQuery<PagedResult<ComplianceRecordDto>?>
{
    public string? Name { get; set; }
}