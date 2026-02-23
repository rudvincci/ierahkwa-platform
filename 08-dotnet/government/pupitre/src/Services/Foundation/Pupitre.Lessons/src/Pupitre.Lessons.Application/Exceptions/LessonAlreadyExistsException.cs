using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Lessons.Domain.Entities;

namespace Pupitre.Lessons.Application.Exceptions;

internal class LessonAlreadyExistsException : MameyException
{
    public LessonAlreadyExistsException(LessonId lessonId)
        : base($"Lesson with ID: '{lessonId.Value}' already exists.")
        => LessonId = lessonId;

    public LessonId LessonId { get; }
}
