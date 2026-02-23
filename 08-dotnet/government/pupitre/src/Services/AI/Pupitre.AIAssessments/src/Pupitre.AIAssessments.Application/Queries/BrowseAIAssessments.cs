using System;
using Mamey.CQRS.Queries;
using Pupitre.AIAssessments.Application.DTO;


namespace Pupitre.AIAssessments.Application.Queries;

internal class BrowseAIAssessments : PagedQueryBase, IQuery<PagedResult<AIAssessmentDto>?>
{
    public string? Name { get; set; }
}