using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Educators.Application.Events.Rejected;
using Pupitre.Educators.Application.Exceptions;
using System;
using Pupitre.Educators.Domain.Exceptions;
using Pupitre.Educators.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Educators.Contracts.Commands;

namespace Pupitre.Educators.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            EducatorAlreadyExistsException ex => message switch
            {
                AddEducator cmd => new AddEducatorRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            EducatorNotFoundException ex => message switch
            {
                UpdateEducator cmd => new UpdateEducatorRejected(ex.EducatorId, ex.Message, ex.Code),
                DeleteEducator cmd => new DeleteEducatorRejected(ex.EducatorId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

