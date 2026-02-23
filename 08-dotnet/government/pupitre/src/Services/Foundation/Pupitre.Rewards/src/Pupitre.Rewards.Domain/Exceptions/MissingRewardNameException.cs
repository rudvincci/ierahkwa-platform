using Mamey.Exceptions;
using System;
using Mamey.Types;

namespace Pupitre.Rewards.Domain.Exceptions;

internal class MissingRewardNameException : DomainException
{
    public MissingRewardNameException()
        : base("Reward name is missing.")
    {
    }

    public override string Code => "missing_reward_name";
}
