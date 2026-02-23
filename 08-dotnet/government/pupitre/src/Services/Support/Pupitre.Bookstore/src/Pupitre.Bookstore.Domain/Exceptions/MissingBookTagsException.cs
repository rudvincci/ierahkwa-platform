using Mamey.Exceptions;

namespace Pupitre.Bookstore.Domain.Exceptions;

internal class MissingBookTagsException : DomainException
{
    public MissingBookTagsException()
        : base("Book tags are missing.")
    {
    }

    public override string Code => "missing_book_tags";
}