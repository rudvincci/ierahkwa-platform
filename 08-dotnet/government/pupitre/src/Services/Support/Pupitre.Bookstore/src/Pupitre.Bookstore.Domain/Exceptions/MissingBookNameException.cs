using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Bookstore.Domain.Exceptions;

internal class MissingBookNameException : DomainException
{
    public MissingBookNameException()
        : base("Book name is missing.")
    {
    }

    public override string Code => "missing_book_name";
}
