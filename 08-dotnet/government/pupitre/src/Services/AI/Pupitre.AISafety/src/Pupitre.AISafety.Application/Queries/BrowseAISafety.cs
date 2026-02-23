using System;
using Mamey.CQRS.Queries;
using Pupitre.AISafety.Application.DTO;


namespace Pupitre.AISafety.Application.Queries;

internal class BrowseAISafety : PagedQueryBase, IQuery<PagedResult<SafetyCheckDto>?>
{
    public string? Name { get; set; }
}