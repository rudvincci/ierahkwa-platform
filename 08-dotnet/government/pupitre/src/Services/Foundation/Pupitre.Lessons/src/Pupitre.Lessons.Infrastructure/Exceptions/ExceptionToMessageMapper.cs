using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Lessons.Application.Events.Rejected;
using Pupitre.Lessons.Application.Exceptions;
using System;
using Pupitre.Lessons.Domain.Exceptions;
using Pupitre.Lessons.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Lessons.Contracts.Commands;

namespace Pupitre.Lessons.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            LessonAlreadyExistsException ex => message switch
            {
                AddLesson cmd => new AddLessonRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            LessonNotFoundException ex => message switch
            {
                UpdateLesson cmd => new UpdateLessonRejected(ex.LessonId, ex.Message, ex.Code),
                DeleteLesson cmd => new DeleteLessonRejected(ex.LessonId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

