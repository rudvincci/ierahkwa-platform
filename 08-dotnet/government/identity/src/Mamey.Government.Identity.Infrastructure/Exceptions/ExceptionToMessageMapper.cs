using System;
using Mamey.MessageBrokers.RabbitMQ;
using Mamey.Government.Identity.Application.Events.Rejected;
using Mamey.Government.Identity.Application.Exceptions;
using System;
using Mamey.Government.Identity.Domain.Exceptions;
using Mamey.Government.Identity.Application.Commands;
using Mamey.Exceptions;
using Mamey.Government.Identity.Contracts.Commands;

namespace Mamey.Government.Identity.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object? Map(Exception exception, object message)
        => exception switch
        {
            SubjectAlreadyExistsException ex => message switch
            {
                AddSubject cmd => new AddSubjectRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null
            },
            SubjectNotFoundException ex => message switch
            {
                UpdateSubject cmd => new UpdateSubjectRejected(ex.SubjectId, ex.Message, ex.Code),
                DeleteSubject cmd => new DeleteSubjectRejected(ex.SubjectId, ex.Message, ex.Code),
                _ => null
            },
            _ => null
        };
}

