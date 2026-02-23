using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Operations.Application.Events.Rejected;
using Pupitre.Operations.Application.Exceptions;
using System;
using Pupitre.Operations.Domain.Exceptions;
using Pupitre.Operations.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Operations.Contracts.Commands;

namespace Pupitre.Operations.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            OperationMetricAlreadyExistsException ex => message switch
            {
                AddOperationMetric cmd => new AddOperationMetricRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            OperationMetricNotFoundException ex => message switch
            {
                UpdateOperationMetric cmd => new UpdateOperationMetricRejected(ex.OperationMetricId, ex.Message, ex.Code),
                DeleteOperationMetric cmd => new DeleteOperationMetricRejected(ex.OperationMetricId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

