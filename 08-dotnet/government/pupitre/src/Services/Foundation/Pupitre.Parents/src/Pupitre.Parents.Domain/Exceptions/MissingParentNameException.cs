using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Parents.Domain.Exceptions;

internal class MissingParentNameException : DomainException
{
    public MissingParentNameException()
        : base("Parent name is missing.")
    {
    }

    public override string Code => "missing_parent_name";
}
