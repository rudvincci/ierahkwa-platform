using System;
using Mamey.CQRS.Queries;
using Pupitre.Operations.Application.DTO;


namespace Pupitre.Operations.Application.Queries;

internal class BrowseOperations : PagedQueryBase, IQuery<PagedResult<OperationMetricDto>?>
{
    public string? Name { get; set; }
}