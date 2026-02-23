using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Curricula.Domain.Exceptions;

internal class MissingCurriculumNameException : DomainException
{
    public MissingCurriculumNameException()
        : base("Curriculum name is missing.")
    {
    }

    public override string Code => "missing_curriculum_name";
}
