using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Analytics.Application.Events.Rejected;
using Pupitre.Analytics.Application.Exceptions;
using System;
using Pupitre.Analytics.Domain.Exceptions;
using Pupitre.Analytics.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Analytics.Contracts.Commands;

namespace Pupitre.Analytics.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            AnalyticAlreadyExistsException ex => message switch
            {
                AddAnalytic cmd => new AddAnalyticRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            AnalyticNotFoundException ex => message switch
            {
                UpdateAnalytic cmd => new UpdateAnalyticRejected(ex.AnalyticId, ex.Message, ex.Code),
                DeleteAnalytic cmd => new DeleteAnalyticRejected(ex.AnalyticId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

