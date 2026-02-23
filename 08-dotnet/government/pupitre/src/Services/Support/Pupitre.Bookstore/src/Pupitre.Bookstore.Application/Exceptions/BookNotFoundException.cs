using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Bookstore.Domain.Entities;

namespace Pupitre.Bookstore.Application.Exceptions;

internal class BookNotFoundException : MameyException
{
    public BookNotFoundException(BookId bookId)
        : base($"Book with ID: '{bookId.Value}' was not found.")
        => BookId = bookId;

    public BookId BookId { get; }
}

