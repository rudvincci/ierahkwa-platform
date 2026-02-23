using System;
using Mamey.CQRS.Queries;
using Pupitre.AISpeech.Application.DTO;


namespace Pupitre.AISpeech.Application.Queries;

internal class BrowseAISpeech : PagedQueryBase, IQuery<PagedResult<SpeechRequestDto>?>
{
    public string? Name { get; set; }
}