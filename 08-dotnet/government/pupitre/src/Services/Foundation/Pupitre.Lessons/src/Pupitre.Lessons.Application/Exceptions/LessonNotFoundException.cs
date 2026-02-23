using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Lessons.Domain.Entities;

namespace Pupitre.Lessons.Application.Exceptions;

internal class LessonNotFoundException : MameyException
{
    public LessonNotFoundException(LessonId lessonId)
        : base($"Lesson with ID: '{lessonId.Value}' was not found.")
        => LessonId = lessonId;

    public LessonId LessonId { get; }
}

