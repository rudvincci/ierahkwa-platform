using System;
using Pupitre.Bookstore.Domain.Entities;
using Mamey.Types;

namespace Pupitre.Bookstore.Domain.Repositories;

internal interface IBookRepository
{
    Task AddAsync(Book book, CancellationToken cancellationToken = default);
    Task UpdateAsync(Book book, CancellationToken cancellationToken = default);
    Task DeleteAsync(BookId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Book>> BrowseAsync(CancellationToken cancellationToken = default);
    Task<Book> GetAsync(BookId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(BookId id, CancellationToken cancellationToken = default);
}
