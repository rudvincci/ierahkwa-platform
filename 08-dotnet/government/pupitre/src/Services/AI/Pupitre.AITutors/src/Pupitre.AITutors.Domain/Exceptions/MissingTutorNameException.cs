using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.AITutors.Domain.Exceptions;

internal class MissingTutorNameException : DomainException
{
    public MissingTutorNameException()
        : base("Tutor name is missing.")
    {
    }

    public override string Code => "missing_tutor_name";
}
