using System;
using Mamey.CQRS.Queries;
using Pupitre.Analytics.Application.DTO;


namespace Pupitre.Analytics.Application.Queries;

internal class BrowseAnalytics : PagedQueryBase, IQuery<PagedResult<AnalyticDto>?>
{
    public string? Name { get; set; }
}