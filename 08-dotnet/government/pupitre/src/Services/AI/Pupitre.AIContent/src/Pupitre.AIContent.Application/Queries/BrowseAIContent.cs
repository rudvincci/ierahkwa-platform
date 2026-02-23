using System;
using Mamey.CQRS.Queries;
using Pupitre.AIContent.Application.DTO;


namespace Pupitre.AIContent.Application.Queries;

internal class BrowseAIContent : PagedQueryBase, IQuery<PagedResult<ContentGenerationDto>?>
{
    public string? Name { get; set; }
}