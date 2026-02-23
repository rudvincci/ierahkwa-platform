using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Bookstore.Domain.Entities;

namespace Pupitre.Bookstore.Application.Exceptions;

internal class BookAlreadyExistsException : MameyException
{
    public BookAlreadyExistsException(BookId bookId)
        : base($"Book with ID: '{bookId.Value}' already exists.")
        => BookId = bookId;

    public BookId BookId { get; }
}
