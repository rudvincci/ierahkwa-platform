using Mamey.Exceptions;

namespace Pupitre.Curricula.Domain.Exceptions;

internal class MissingCurriculumTagsException : DomainException
{
    public MissingCurriculumTagsException()
        : base("Curriculum tags are missing.")
    {
    }

    public override string Code => "missing_curriculum_tags";
}