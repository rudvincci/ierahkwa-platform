using System;
using Mamey.CQRS.Queries;
using Pupitre.Bookstore.Application.DTO;


namespace Pupitre.Bookstore.Application.Queries;

internal class BrowseBookstore : PagedQueryBase, IQuery<PagedResult<BookDto>?>
{
    public string? Name { get; set; }
}