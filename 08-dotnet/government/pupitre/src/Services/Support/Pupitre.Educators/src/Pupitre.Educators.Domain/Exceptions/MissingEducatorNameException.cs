using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Educators.Domain.Exceptions;

internal class MissingEducatorNameException : DomainException
{
    public MissingEducatorNameException()
        : base("Educator name is missing.")
    {
    }

    public override string Code => "missing_educator_name";
}
