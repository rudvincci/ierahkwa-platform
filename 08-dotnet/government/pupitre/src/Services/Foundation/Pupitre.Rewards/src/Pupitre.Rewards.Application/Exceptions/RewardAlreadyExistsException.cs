using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Rewards.Domain.Entities;

namespace Pupitre.Rewards.Application.Exceptions;

internal class RewardAlreadyExistsException : MameyException
{
    public RewardAlreadyExistsException(RewardId rewardId)
        : base($"Reward with ID: '{rewardId.Value}' already exists.")
        => RewardId = rewardId;

    public RewardId RewardId { get; }
}
