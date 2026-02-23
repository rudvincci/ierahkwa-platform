using System;
using Mamey.MessageBrokers.RabbitMQ;
using Pupitre.Rewards.Application.Events.Rejected;
using Pupitre.Rewards.Application.Exceptions;
using System;
using Pupitre.Rewards.Domain.Exceptions;
using Pupitre.Rewards.Application.Commands;
using Mamey.Exceptions;
using Pupitre.Rewards.Contracts.Commands;

namespace Pupitre.Rewards.Infrastructure.Exceptions;

internal sealed class ExceptionToMessageMapper : IExceptionToMessageMapper
{
    public object Map(Exception exception, object message)
        => exception switch
        {
            RewardAlreadyExistsException ex => message switch
            {
                AddReward cmd => new AddRewardRejected(cmd.Id, ex.Reason, ex.Code),
                _ => null!
            },
            RewardNotFoundException ex => message switch
            {
                UpdateReward cmd => new UpdateRewardRejected(ex.RewardId, ex.Message, ex.Code),
                DeleteReward cmd => new DeleteRewardRejected(ex.RewardId, ex.Message, ex.Code),
                _ => null!
            },
            _ => null!
        };
}

