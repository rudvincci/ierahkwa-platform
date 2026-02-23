using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Curricula.Application.Events.Rejected;
using Pupitre.Curricula.Application.Exceptions;
using System;
using Pupitre.Curricula.Domain.Exceptions;
using Pupitre.Curricula.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Curricula.Contracts.Commands;

namespace Pupitre.Curricula.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            CurriculumAlreadyExistsException ex => message switch
            {
                AddCurriculum cmd => new AddCurriculumRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            CurriculumNotFoundException ex => message switch
            {
                UpdateCurriculum cmd => new UpdateCurriculumRejected(ex.CurriculumId, ex.Message, ex.Code),
                DeleteCurriculum cmd => new DeleteCurriculumRejected(ex.CurriculumId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

