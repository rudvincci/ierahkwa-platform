using System;
using Mamey.CQRS.Queries;
using Pupitre.Assessments.Application.DTO;


namespace Pupitre.Assessments.Application.Queries;

internal class BrowseAssessments : PagedQueryBase, IQuery<PagedResult<AssessmentDto>?>
{
    public string? Name { get; set; }
}