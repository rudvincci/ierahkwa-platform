using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.AISafety.Domain.Exceptions;

internal class MissingSafetyCheckNameException : DomainException
{
    public MissingSafetyCheckNameException()
        : base("SafetyCheck name is missing.")
    {
    }

    public override string Code => "missing_safetycheck_name";
}
