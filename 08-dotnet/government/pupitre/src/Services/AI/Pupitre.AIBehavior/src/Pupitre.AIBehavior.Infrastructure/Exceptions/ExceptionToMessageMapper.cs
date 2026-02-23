using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.AIBehavior.Application.Events.Rejected;
using Pupitre.AIBehavior.Application.Exceptions;
using System;
using Pupitre.AIBehavior.Domain.Exceptions;
using Pupitre.AIBehavior.Application.Commands;
using Mamey.Exceptions;
using Pupitre.AIBehavior.Contracts.Commands;

namespace Pupitre.AIBehavior.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            BehaviorAlreadyExistsException ex => message switch
            {
                AddBehavior cmd => new AddBehaviorRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            BehaviorNotFoundException ex => message switch
            {
                UpdateBehavior cmd => new UpdateBehaviorRejected(ex.BehaviorId, ex.Message, ex.Code),
                DeleteBehavior cmd => new DeleteBehaviorRejected(ex.BehaviorId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

