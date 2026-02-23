using Mamey.Exceptions;

namespace Pupitre.Lessons.Domain.Exceptions;

internal class MissingLessonTagsException : DomainException
{
    public MissingLessonTagsException()
        : base("Lesson tags are missing.")
    {
    }

    public override string Code => "missing_lesson_tags";
}