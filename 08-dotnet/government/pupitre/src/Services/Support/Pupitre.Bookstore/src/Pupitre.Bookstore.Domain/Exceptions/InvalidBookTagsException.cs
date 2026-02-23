using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Bookstore.Domain.Exceptions;

internal class InvalidBookTagsException : DomainException
{
    public override string Code { get; } = "invalid_book_tags";

    public InvalidBookTagsException() : base("Book tags are invalid.")
    {
    }
}
