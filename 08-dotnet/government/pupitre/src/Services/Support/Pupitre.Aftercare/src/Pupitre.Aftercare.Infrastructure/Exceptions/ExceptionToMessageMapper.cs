using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Aftercare.Application.Events.Rejected;
using Pupitre.Aftercare.Application.Exceptions;
using System;
using Pupitre.Aftercare.Domain.Exceptions;
using Pupitre.Aftercare.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Aftercare.Contracts.Commands;

namespace Pupitre.Aftercare.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            AftercarePlanAlreadyExistsException ex => message switch
            {
                AddAftercarePlan cmd => new AddAftercarePlanRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            AftercarePlanNotFoundException ex => message switch
            {
                UpdateAftercarePlan cmd => new UpdateAftercarePlanRejected(ex.AftercarePlanId, ex.Message, ex.Code),
                DeleteAftercarePlan cmd => new DeleteAftercarePlanRejected(ex.AftercarePlanId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

