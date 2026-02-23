using System;
using Mamey.CQRS.Queries;
using Pupitre.AITranslation.Application.DTO;


namespace Pupitre.AITranslation.Application.Queries;

internal class BrowseAITranslation : PagedQueryBase, IQuery<PagedResult<TranslationRequestDto>?>
{
    public string? Name { get; set; }
}