using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Accessibility.Domain.Exceptions;

internal class MissingAccessProfileNameException : DomainException
{
    public MissingAccessProfileNameException()
        : base("AccessProfile name is missing.")
    {
    }

    public override string Code => "missing_accessprofile_name";
}
