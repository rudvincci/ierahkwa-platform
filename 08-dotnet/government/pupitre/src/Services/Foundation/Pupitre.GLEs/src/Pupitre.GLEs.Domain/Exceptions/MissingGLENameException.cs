using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.GLEs.Domain.Exceptions;

internal class MissingGLENameException : DomainException
{
    public MissingGLENameException()
        : base("GLE name is missing.")
    {
    }

    public override string Code => "missing_gle_name";
}
