using Mamey.Exceptions;

namespace Pupitre.Rewards.Domain.Exceptions;

internal class MissingRewardTagsException : DomainException
{
    public MissingRewardTagsException()
        : base("Reward tags are missing.")
    {
    }

    public override string Code => "missing_reward_tags";
}