using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Curricula.Domain.Exceptions;

internal class InvalidCurriculumTagsException : DomainException
{
    public override string Code { get; } = "invalid_curriculum_tags";

    public InvalidCurriculumTagsException() : base("Curriculum tags are invalid.")
    {
    }
}
