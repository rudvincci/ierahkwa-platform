using Mamey.Exceptions;
using Mamey.Types;
using Pupitre.Rewards.Domain.Entities;

namespace Pupitre.Rewards.Application.Exceptions;

internal class RewardNotFoundException : MameyException
{
    public RewardNotFoundException(RewardId rewardId)
        : base($"Reward with ID: '{rewardId.Value}' was not found.")
        => RewardId = rewardId;

    public RewardId RewardId { get; }
}

