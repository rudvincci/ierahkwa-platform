using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Lessons.Domain.Exceptions;

internal class MissingLessonNameException : DomainException
{
    public MissingLessonNameException()
        : base("Lesson name is missing.")
    {
    }

    public override string Code => "missing_lesson_name";
}
