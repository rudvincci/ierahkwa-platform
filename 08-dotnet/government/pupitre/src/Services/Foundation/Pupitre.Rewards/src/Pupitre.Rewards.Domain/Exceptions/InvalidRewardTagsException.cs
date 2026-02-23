using Mamey.Exceptions;
using Mamey.Types;

namespace Pupitre.Rewards.Domain.Exceptions;

internal class InvalidRewardTagsException : DomainException
{
    public override string Code { get; } = "invalid_reward_tags";

    public InvalidRewardTagsException() : base("Reward tags are invalid.")
    {
    }
}
