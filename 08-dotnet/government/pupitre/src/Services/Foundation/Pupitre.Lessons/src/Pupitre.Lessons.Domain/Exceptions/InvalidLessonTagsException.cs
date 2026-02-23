using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Lessons.Domain.Exceptions;

internal class InvalidLessonTagsException : DomainException
{
    public override string Code { get; } = "invalid_lesson_tags";

    public InvalidLessonTagsException() : base("Lesson tags are invalid.")
    {
    }
}
