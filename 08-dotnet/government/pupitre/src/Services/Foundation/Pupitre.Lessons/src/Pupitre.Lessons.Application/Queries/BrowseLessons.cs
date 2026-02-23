using System;
using Mamey.CQRS.Queries;
using Pupitre.Lessons.Application.DTO;


namespace Pupitre.Lessons.Application.Queries;

internal class BrowseLessons : PagedQueryBase, IQuery<PagedResult<LessonDto>?>
{
    public string? Name { get; set; }
}